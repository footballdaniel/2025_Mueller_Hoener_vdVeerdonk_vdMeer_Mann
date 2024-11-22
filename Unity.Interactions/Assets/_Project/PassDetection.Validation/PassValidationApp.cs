using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Unity.Sentis;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace _Project.PassDetection.Validation
{
	public class PassValidationApp : MonoBehaviour
	{
		[SerializeField]
		ModelAsset _modelAsset;

		void Start()
		{
			var dataset = new FileInfo(Path.Combine(Application.dataPath, "../../Python.PassDetection/dataset.json"));

			if (!dataset.Exists)
				throw new FileNotFoundException("Dataset file not found.");

			_sampleEnumerator = SampleDeserializer.DeserializeSamples(dataset.FullName).GetEnumerator();

			var model = ModelLoader.Load(_modelAsset);
			_worker = new Worker(model, BackendType.GPUCompute);
		}

		public float EvaluateNext()
		{
			JumpToNextNonNullProbability(out var currentSample);

			var input = currentSample!.Inference.ToTensor();

			var stopwatch = Stopwatch.StartNew();
			_worker.SetInput("input", input);
			_worker.Schedule();
			var outputTensor = _worker.PeekOutput("output") as Tensor<float>;
			var cpuOutputTensor = outputTensor.ReadbackAndClone();
			var result = cpuOutputTensor[0];
			cpuOutputTensor.Dispose();
			stopwatch.Stop();

			Debug.Log("Inference Time: " + stopwatch.ElapsedMilliseconds + "ms");
			Debug.Log($"Python Prediction {currentSample.Inference.PassProbability:F3} Unity Prediction {result:F3}");

			input?.Dispose();

			return result;
		}

		void JumpToNextNonNullProbability(out Sample currentSample)
		{
			var probability = 0f;
			const float epsilon = 1e-6f;
			currentSample = _sampleEnumerator.Current;

			while (_sampleEnumerator.MoveNext() && Math.Abs(probability) < epsilon)
			{
				currentSample = _sampleEnumerator.Current;
				probability = currentSample!.Inference.PassProbability;
			}
		}


		void OnDestroy()
		{
			_worker.Dispose();
		}


		IEnumerator<Sample> _sampleEnumerator;
		Worker _worker;
	}
}
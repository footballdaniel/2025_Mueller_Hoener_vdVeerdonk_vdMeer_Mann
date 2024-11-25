using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using _Project.PassDetection.Validation;
using Unity.Sentis;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PassDetection.Valwidation
{
	public class PassValidationApp : MonoBehaviour
	{
		[SerializeField] ModelAsset _modelAsset;
		public string CurrentPrediction => _currentPrediction;

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
			var prediction = cpuOutputTensor[0];
			cpuOutputTensor.Dispose();
			stopwatch.Stop();

			_currentPrediction = "Inference Time: " + stopwatch.ElapsedMilliseconds + " ms" +
			                     "Python Prediction: " + currentSample.Inference.PassProbability.ToString("F3") +
			                     "Unity Prediction: " + prediction.ToString("F3") +
			                     "Label: " + currentSample.Inference.OutcomeLabel;

			Debug.Log(_currentPrediction);

			input?.Dispose();

			return prediction;
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

		string _currentPrediction;


		IEnumerator<Sample> _sampleEnumerator;
		Worker _worker;
	}
}
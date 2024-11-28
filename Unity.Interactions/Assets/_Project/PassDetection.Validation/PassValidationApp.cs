using System.Collections.Generic;
using Tactive.MachineLearning.Models;
using Unity.Sentis;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PassDetection.Validation
{
	public class PassValidationApp : MonoBehaviour
	{
		[SerializeField] ModelAssetWithMetadata _modelWithMetadata;

		Worker _worker;

		void OnDestroy()
		{
			_worker.Dispose();
		}

		public float EvaluateNext()
		{
			var asset = Resources.Load<ModelAsset>(_modelWithMetadata.ModelAssetPath);
			var model = ModelLoader.Load(asset);
			
			_worker = new Worker(model, BackendType.GPUCompute);
			var shape = new TensorShape(_modelWithMetadata.ExampleInput[0].Length, _modelWithMetadata.ExampleInput[1].Length, _modelWithMetadata.ExampleInput[2].Length);

			var exampleInput = _modelWithMetadata.ExampleInput;
			var flattenedValues = new List<float>();

			foreach (var example in exampleInput)
			foreach (var example2 in example)
			foreach (var example3 in example2)
				flattenedValues.Add(example3);

			var input = new Tensor<float>(shape, flattenedValues.ToArray());

			_worker.Schedule(input);
			var outputTensor = _worker.PeekOutput() as Tensor<float>;
			var cpuOutputTensor = outputTensor.ReadbackAndClone();

			var result = cpuOutputTensor[0];
			cpuOutputTensor.Dispose();
			input.Dispose();

			Debug.Log("Result: " + result);
			Debug.Log("Original" + _modelWithMetadata.ExampleOutput);
			return result;
		}
	}
}
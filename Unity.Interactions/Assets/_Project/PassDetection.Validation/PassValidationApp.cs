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
			var shape = new TensorShape(_modelWithMetadata.InputShape.ToArray());
			var input = new Tensor<float>(shape, _modelWithMetadata.SampleInput.ToArray());

			_worker.Schedule(input);
			var outputTensor = _worker.PeekOutput() as Tensor<float>;
			var cpuOutputTensor = outputTensor.ReadbackAndClone();

			var result = cpuOutputTensor[0];
			cpuOutputTensor.Dispose();
			input.Dispose();

			Debug.Log("Result: " + result);
			Debug.Log("Original" + _modelWithMetadata.SampleOutput);
			return result;
		}
	}
}
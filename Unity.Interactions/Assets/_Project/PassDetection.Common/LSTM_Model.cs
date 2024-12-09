using System;
using System.Collections.Generic;
using _Project.PassDetection.Common;
using Tactive.MachineLearning.Features;
using Tactive.MachineLearning.Models;
using Unity.Sentis;
using UnityEngine;

namespace PassDetection.Replay
{
	public class LstmModel : IDisposable
	{
		public LstmModel(ModelAssetWithMetadata assetWithMetadata)
		{
			var asset = Resources.Load<ModelAsset>(assetWithMetadata.ModelAssetPath);
			var model = ModelLoader.Load(asset);

			_featureEngineer = new FeatureEngineer<InputData>();
			foreach (var featureName in assetWithMetadata.FeatureNames)
				_featureEngineer.AddFeature(FeatureRegistry.Create<InputData>(featureName));
			
			_worker = new Worker(model, BackendType.GPUCompute);
		}

		public void Dispose()
		{
			_worker?.Dispose();
			
		}


		public float Evaluate(InputData data)
		{
			var flattenedValues = new List<float>();
				flattenedValues.AddRange(_featureEngineer.Engineer(data));

			var timeseriesCount = 10;
			var featureCount = _featureEngineer.FeatureSize;
			var batchSize = 1;

			var shape = new TensorShape(batchSize, timeseriesCount, featureCount);
			var input = new Tensor<float>(shape, flattenedValues.ToArray());

			_worker.Schedule(input);
			var outputTensor = _worker.PeekOutput() as Tensor<float>;
			var cpuOutputTensor = outputTensor.ReadbackAndClone();

			var result = cpuOutputTensor[0];
			cpuOutputTensor.Dispose();
			input.Dispose();
			outputTensor.Dispose();

			return result;
		}

		readonly Worker _worker;
		readonly FeatureEngineer<InputData> _featureEngineer;
	}
}
using System;
using System.Collections.Generic;
using PassDetection.Replay.Features;
using Tactive.MachineLearning._Project.MachineLearning;
using Tactive.MachineLearning.Features;
using Tactive.MachineLearning.Models;
using Unity.Sentis;

namespace PassDetection.Replay
{
	public class LstmModel : IDisposable
	{
		public LstmModel(ModelAssetWithMetadata asset)
		{
			var model = ModelLoaderWithMetadata.Load(asset);

			_features = new List<BaseFeature<InputData>>();
			foreach (var featureName in asset.FeatureNames)
				_features.Add(FeatureRegistry.Create<InputData>(featureName));
			
			_worker = new Worker(model, BackendType.GPUCompute);
		}

		public void Dispose()
		{
			_worker?.Dispose();
		}


		public float Evaluate(InputData data)
		{
			var targets = new List<Target>();
			
			foreach (var feature in _features)
				targets.AddRange(feature.Calculate(data));

			var timeseriesCount = targets[0].Values.Count;
			var featureCount = targets.Count;
			var batchSize = 1;

			var shape = new TensorShape(batchSize, timeseriesCount, featureCount);
			var flattenedValues = new float[timeseriesCount * featureCount];

			var index = 0;

			foreach (var feature in targets)
			foreach (var value in feature.Values)
				flattenedValues[index++] = value;

			var input = new Tensor<float>(shape, flattenedValues);

			_worker.Schedule(input);
			var outputTensor = _worker.PeekOutput() as Tensor<float>;
			var cpuOutputTensor = outputTensor.ReadbackAndClone();

			var result = cpuOutputTensor[0];
			cpuOutputTensor.Dispose();
			input.Dispose();

			return result;
		}

		readonly Worker _worker;
		readonly List<BaseFeature<InputData>> _features;
	}
}
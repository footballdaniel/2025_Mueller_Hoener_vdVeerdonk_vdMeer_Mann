using System;
using System.Linq;
using _Project.Interactions.Scripts.Domain;
using Unity.Sentis;
using Debug = UnityEngine.Debug;

namespace PassDetection
{
	public class LSTM_Model : IDisposable
	{
		public LSTM_Model(ModelAsset asset, Trial trial)
		{
			var model = ModelLoader.Load(asset);
			_worker = new Worker(model, BackendType.GPUCompute);
			_trial = trial;
		}

		public void Dispose()
		{
			_input?.Dispose();
			_worker?.Dispose();
		}


		public float EvaluateTrialAtTime(float timeSinceTrialStart)
		{
			var features0 = ZeroedPositionDominantFootCalculator.Calculate(_trial, timeSinceTrialStart);
			var features1 = OffsetDominantFootToNonDominantFootCalculator.Calculate(_trial, timeSinceTrialStart);
			var features2 = VelocitiesDominantFootCalculator.Calculate(_trial, timeSinceTrialStart);
			var features3 = VelocitiesNonDominantFootCalculator.Calculate(_trial, timeSinceTrialStart);


			var allFeatures = features0.Concat(features1).Concat(features2).Concat(features3).ToList();
			var numTimeSteps = allFeatures.First().Values.Count;
			var numFeatures = allFeatures.Count;

			var allFeatureValues = allFeatures.SelectMany(feature => feature.Values).ToList();
			var requiredSize = numTimeSteps * numFeatures;

			if (allFeatureValues.Count < requiredSize)
				Debug.LogWarning("Not enough features to fill the input tensor. Padding with zeros.");
			else if (allFeatureValues.Count > requiredSize)
				Debug.LogWarning("Too many features to fill the input tensor");

			var featureArray = allFeatureValues.ToArray();
			_input = new Tensor<float>(new TensorShape(1, numTimeSteps, numFeatures), featureArray);

			_worker.Schedule(_input);
			var outputTensor = _worker.PeekOutput("output") as Tensor<float>;
			var cpuOutputTensor = outputTensor.ReadbackAndClone();

			var result = cpuOutputTensor[0];
			cpuOutputTensor.Dispose();
			_input.Dispose();

			return result;
		}

		readonly ModelAsset _asset;

		Tensor _input;
		Trial _trial;
		Worker _worker;
	}
}
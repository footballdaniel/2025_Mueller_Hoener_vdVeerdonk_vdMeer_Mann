using System.Diagnostics;
using System.Linq;
using _Project.Interactions.Scripts.Domain;
using Unity.Sentis;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PassDetection
{
	public class LSTM_Model : MonoBehaviour
	{
		[SerializeField]
		ModelAsset _modelAsset;


		void Start()
		{
			var model = ModelLoader.Load(_modelAsset);
			_worker = new Worker(model, BackendType.GPUCompute);
		}

		public void Bind(Trial trial)
		{
			_trial = trial;
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


		void OnDestroy()
		{
			_input.Dispose();
			_worker.Dispose();
		}

		Tensor _input;
		Trial _trial;
		Worker _worker;
	}
}
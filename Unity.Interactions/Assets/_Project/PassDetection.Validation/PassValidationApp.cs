using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using _Project.PassDetection.Common;
using _Project.PassDetection.Validation;
using PassDetection.Replay;
using Tactive.MachineLearning.Models;
using Unity.Sentis;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PassDetection.Validation
{
	public class PassValidationApp : MonoBehaviour
	{
		[SerializeField] ModelAssetWithMetadata _modelWithMetadata;
		public string CurrentPrediction => _currentPrediction;

		void Start()
		{
			var dataset = new FileInfo(Path.Combine(Application.dataPath, "../../Python.PassDetection/dataset.json"));

			if (!dataset.Exists)
				throw new FileNotFoundException("Dataset file not found.");

			_sampleEnumerator = SampleDeserializer.DeserializeSamples(dataset.FullName).GetEnumerator();

			
			_lstmModel = new LstmModel(_modelWithMetadata);
		}

		public float EvaluateNext()
		{
			// JumpToNextNonNullProbability(out var currentSample);
			_sampleEnumerator.MoveNext();
			var currentSample = _sampleEnumerator.Current;


			var input = new InputData(
				currentSample!.Recording.InputData.UserDominantFootPositions,
				currentSample.Recording.InputData.UserNonDominantFootPositions,
				currentSample.Recording.InputData.Timestamps
			);
			
			var stopwatch = Stopwatch.StartNew();
			var prediction = _lstmModel.Evaluate(input);
			stopwatch.Stop();

			_currentPrediction = "Inference Time: " + stopwatch.ElapsedMilliseconds + " ms" +
			                     "Python Prediction: " + currentSample.Inference.PassProbability.ToString("F3") +
			                     "Unity Prediction: " + prediction.ToString("F3") +
			                     "Label: " + currentSample.Inference.OutcomeLabel;

			Debug.Log(_currentPrediction);
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
		LstmModel _lstmModel;
	}
}
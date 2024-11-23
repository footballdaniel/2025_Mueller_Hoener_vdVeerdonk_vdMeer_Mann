using System.Collections.Generic;
using System.IO;
using _Project.Interactions.Scripts.Common;
using _Project.Interactions.Scripts.Domain;
using Newtonsoft.Json;
using PassDetection.Replay.Features;
using Tactive.MachineLearning.Models;
using UnityEditor;
using UnityEngine;

namespace PassDetection.Replay
{
	public static class LstmModelWrapper
	{
		[MenuItem("TEST/Run LSTM Model Predictions")]
		public static void RunPredictions()
		{
			var jsonPath = "C:/Users/danie/Desktop/git/2025_Mueller_Hoener_Mann/Data/Pilot_4/trial_3_2024-10-29_15-58-40.json";
			var modelPath = "pass_detection_model_with_metadata";
			var outputPath = "C:/Users/danie/Desktop/git/2025_Mueller_Hoener_Mann/Data/Pilot_4/trial_3_2024-10-29_15-58-40_recalculated.csv";

			var asset = Resources.Load<ModelAssetWithMetadata>(modelPath);
			var json = File.ReadAllText(jsonPath);
			var jsonSettings = new JsonSerializerSettings();
			jsonSettings.Converters.Add(new Vector3Converter());
			jsonSettings.Converters.Add(new SideEnumConverter());
			var trial = JsonConvert.DeserializeObject<Trial>(json, jsonSettings);

			var model = new LstmModel(asset);

			var predictions = new List<string> { "Timestamp,Prediction" }; // CSV header

			foreach (var timestamp in trial.Timestamps)
			{
				var toTake = 10;

				if (trial.Timestamps.IndexOf(timestamp) + toTake > trial.Timestamps.Count)
					break;

				var next10Timestamps = trial.Timestamps.GetRange(trial.Timestamps.IndexOf(timestamp), toTake);
				var next10Positions = trial.UserDominantFootPositions.GetRange(trial.Timestamps.IndexOf(timestamp), toTake);
				var next10NonDominantPositions = trial.UserNonDominantFootPositions.GetRange(trial.Timestamps.IndexOf(timestamp), toTake);

				var inputData = new InputData(next10Positions, next10NonDominantPositions, next10Timestamps);
				var prediction = model.Evaluate(inputData);
				predictions.Add($"{timestamp:F2},{prediction:F2}");
			}

			File.WriteAllLines(outputPath, predictions);

			model.Dispose();
		}
	}
}
using System.Collections.Generic;
using System.IO;
using _Project.Interactions.Scripts.Common;
using _Project.Interactions.Scripts.Domain;
using Newtonsoft.Json;
using Unity.Sentis;
using UnityEditor;
using UnityEngine;

namespace PassDetection
{
	public class LSTM_ModelWrapper
	{
		[MenuItem("TEST/Run LSTM Model Predictions")]
		public static void RunPredictions()
		{
			var jsonPath = "C:/Users/danie/Desktop/git/2025_Mueller_Hoener_Mann/Data/Pilot_4/trial_3_2024-10-29_15-58-40.json";
			var modelPath = "pass_detection_model"; 
			var outputPath = "C:/Users/danie/Desktop/git/2025_Mueller_Hoener_Mann/Data/Pilot_4/trial_3_2024-10-29_15-58-40.csv";

			var asset = Resources.Load<ModelAsset>(modelPath);
			var json = File.ReadAllText(jsonPath);
			var jsonSettings = new JsonSerializerSettings();
			jsonSettings.Converters.Add(new Vector3Converter());
			jsonSettings.Converters.Add(new SideEnumConverter());
			var trial = JsonConvert.DeserializeObject<Trial>(json, jsonSettings);

			var model = new LSTM_Model(asset, trial);

			var predictions = new List<string> { "Timestamp,Prediction" }; // CSV header

			foreach (var timestamp in trial.Timestamps)
			{
				var prediction = model.EvaluateTrialAtTime(timestamp);
				predictions.Add($"{timestamp.ToString("F2")},{prediction.ToString("F2")}");
			}

			File.WriteAllLines(outputPath, predictions);

			model.Dispose();
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Domain
{
	public class Trial
	{
		readonly List<float> _timestamps;

		public Trial(int trialNumber)
		{
			_timestamps = new List<float>();
			TrialNumber = trialNumber;
		}

		public int NumberOfFrames => _timestamps.Count;
		public int TrialNumber { get; }
		public float Duration { get; private set; }
		public List<Vector3> OpponentHipPositions { get; } = new();
		public List<Vector3> UserHeadPositions { get; set; } = new();
		public List<Vector3> UserDominantFootPositions { get; set; } = new();
		public List<Vector3> UserNonDominantFootPositions { get; set; } = new();

		public void Tick(float deltaTime)
		{
			Duration += deltaTime;
			_timestamps.Add(Duration);
		}

		public void Save()
		{
			var jsonSettings = new JsonSerializerSettings();
			jsonSettings.Converters.Add(new Vector3Converter());
			jsonSettings.Formatting = Formatting.Indented;

			var jsonData = JsonConvert.SerializeObject(this, jsonSettings);
			var fileNameWithDateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
			var path = Path.Combine(Application.persistentDataPath, $"trial_{TrialNumber}_{fileNameWithDateTime}.json");
			File.WriteAllText(path, jsonData);
		}
	}
}
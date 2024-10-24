using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Domain
{
	public class Trial
	{
		readonly List<float> _timestamps;

		public Trial(float startTime)
		{
			_timestamps = new List<float>();
			StartTime = startTime;
		}

		public List<Vector3> OpponentHipPositions { get; } = new();
		public float StartTime { get; }
		public float Duration { get; private set; }
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

			var jsonData = JsonConvert.SerializeObject(this, jsonSettings);
			var path = Path.Combine(Application.persistentDataPath, "trial_data.json");
			File.WriteAllText(path, jsonData);
		}
	}
}
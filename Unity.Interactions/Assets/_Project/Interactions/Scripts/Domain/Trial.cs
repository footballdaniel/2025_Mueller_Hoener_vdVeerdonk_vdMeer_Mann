using System;
using System.Collections.Generic;
using System.IO;
using Interactions.Common;
using Newtonsoft.Json;
using UnityEngine;

namespace Interactions.Domain
{
	public class Trial
	{

		public Trial(int trialNumber, int frameRateHz, Side dominantFoot)
		{
			Timestamps = new List<float>();
			TrialNumber = trialNumber;
			FrameRateHz = frameRateHz;
			DominantFoot = dominantFoot;
		}

		public Side DominantFoot { get; private set; }
		public int FrameRateHz { get; private set; }
		public int NumberOfFrames => Timestamps.Count;
		public List<float> Timestamps { get; }
		public int TrialNumber { get; }
		public float Duration { get; private set; }
		public List<BallEvent> BallEvents { get; } = new();
		public List<Vector3> OpponentHipPositions { get; } = new();
		public List<Vector3> UserHeadPositions { get; set; } = new();
		public List<Vector3> UserDominantFootPositions { get; set; } = new();
		public List<Vector3> UserNonDominantFootPositions { get; set; } = new();
		public List<Vector3> UserHipPositions { get; set; } = new();

		public void Save()
		{
			var jsonSettings = new JsonSerializerSettings();
			jsonSettings.Converters.Add(new Vector3Converter());
			jsonSettings.Converters.Add(new SideEnumConverter());
			jsonSettings.Formatting = Formatting.Indented;

			var jsonData = JsonConvert.SerializeObject(this, jsonSettings);
			var fileNameWithDateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
			var path = Path.Combine(Application.persistentDataPath, $"trial_{TrialNumber}_{fileNameWithDateTime}.json");
			File.WriteAllText(path, jsonData);
		}

		public void Tick(float deltaTime)
		{
			Duration += deltaTime;
			Timestamps.Add(Duration);
		}
	}

}
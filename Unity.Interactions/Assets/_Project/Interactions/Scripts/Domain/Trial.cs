using System;
using System.Collections.Generic;
using System.IO;
using Interactions.Apps;
using Interactions.Common;
using Newtonsoft.Json;
using UnityEngine;

namespace Interactions.Domain
{
	public class NoTrial : Trial
	{

		public NoTrial() : base(-1,-1, Side.RIGHT, ExperimentalCondition.InSitu)
		{
		}
	}
	
	
	public class Trial
	{

		public Trial(int trialNumber, int frameRateHz, Side dominantFoot, ExperimentalCondition condition)
		{
			Timestamps = new List<float>();
			TrialNumber = trialNumber;
			FrameRateHz = frameRateHz;
			DominantFoot = dominantFoot;
			Condition = condition;
		}

		public ExperimentalCondition Condition { get; set; }
		public Side DominantFoot { get; private set; }
		public int FrameRateHz { get; private set; }
		public int NumberOfFrames => Timestamps.Count;
		public bool ContainsError { get; set; }
		public List<float> Timestamps { get; }
		public int TrialNumber { get; }
		public float Duration { get; private set; }
		public List<BallEvent> BallEvents { get; } = new();
		public List<Vector3> UserHeadPositions { get; set; } = new();
		public List<Vector3> UserDominantFootPositions { get; set; } = new();
		public List<Vector3> UserNonDominantFootPositions { get; set; } = new();
		public List<Vector3> UserHipPositions { get; set; } = new();
		public List<Vector3> OpponentHipPositions { get; } = new();


		public void Save()
		{
			var jsonSettings = new JsonSerializerSettings();
			jsonSettings.Converters.Add(new Vector3Converter());
			jsonSettings.Converters.Add(new SideEnumConverter());
			jsonSettings.Formatting = Formatting.Indented;

			var jsonData = JsonConvert.SerializeObject(this, jsonSettings);
			var fileNameWithDateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
			var path = Path.Combine(Application.persistentDataPath, $"trial_{TrialNumber}_{Condition.ToString()}_{fileNameWithDateTime}.json");
			File.WriteAllText(path, jsonData);
		}

		public void Tick(float deltaTime)
		{
			Duration += deltaTime;
			Timestamps.Add(Duration);
		}
	}

}
using System.Collections.Generic;
using Newtonsoft.Json;
using PassDetection.Validation;
using UnityEngine;

namespace _Project.PassDetection.Validation
{
	public enum Foot
	{
		Unassigned = 0,
		Right = 1,
		Left = 2
	}

	public class PassEvent
	{
		[JsonProperty("is_a_pass")]
		public bool IsAPass { get; set; }

		[JsonProperty("frame_number")]
		public int FrameNumber { get; set; }

		[JsonProperty("foot")]
		public Foot Foot { get; set; }

		[JsonProperty("pass_id")]
		public int PassId { get; set; }

		[JsonProperty("timestamp")]
		public float Timestamp { get; set; }
	}

	public class Position
	{
		[JsonProperty("x")]
		public float X { get; set; }

		[JsonProperty("y")]
		public float Y { get; set; }
	
		[JsonProperty("z")]
		public float Z { get; set; }
	}

	public class InputDataDto
	{
		[JsonProperty("user_dominant_foot_positions")]
		public List<Vector3> UserDominantFootPositions { get; set; }

		[JsonProperty("user_non_dominant_foot_positions")]
		public List<Vector3> UserNonDominantFootPositions { get; set; }

		[JsonProperty("timestamps")]
		public List<float> Timestamps { get; set; }
	}

	public class Recording
	{
		[JsonProperty("input_data")]
		public InputDataDto InputData { get; set; }
		
		[JsonProperty("frame_rate_hz")]
		public int FrameRateHz { get; set; }

		[JsonProperty("number_of_frames")]
		public int NumberOfFrames { get; set; }

		[JsonProperty("trial_number")]
		public int TrialNumber { get; set; }

		[JsonProperty("duration")]
		public float Duration { get; set; }

		[JsonProperty("pass_events")]
		public List<PassEvent> PassEvents { get; set; }
	}

	public class Target
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("values")]
		public List<float> Values { get; set; }
	}

	public class Inference
	{
		[JsonProperty("targets")]
		public List<Target> Targets { get; set; }

		[JsonProperty("outcome_label")]
		public int OutcomeLabel { get; set; }

		[JsonProperty("pass_probability")]
		public float PassProbability { get; set; }

		[JsonProperty("split")]
		[JsonConverter(typeof(SplitEnumConverter))]
		public Split Split { get; set; }
	}

	public enum Split
	{
		UNASSIGNED = 0,
		TRAIN = 1,
		VALIDATION = 2,
		TEST = 3
	}

	public class Augmentation
	{
		[JsonProperty("rotation_angle")]
		public float RotationAngle { get; set; }

		[JsonProperty("swapped_feet")]
		public bool SwappedFeet { get; set; }
	}

	public class Sample
	{
		[JsonProperty("recording")]
		public Recording Recording { get; set; }

		[JsonProperty("pass_event")]
		public PassEvent PassEvent { get; set; }

		[JsonProperty("inference")]
		public Inference Inference { get; set; }

		[JsonProperty("augmentation")]
		public Augmentation Augmentation { get; set; }
	}
}
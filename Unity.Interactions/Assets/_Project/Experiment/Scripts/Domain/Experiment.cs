using System;
using Interactions.Apps;
using Interactions.Domain.Goals;
using Interactions.Domain.Opponents;
using Interactions.Domain.VideoRecorders;
using Interactions.Infra;
using UnityEngine;

namespace Interactions.Domain
{
	[Serializable]
	public class Experiment
	{
		[field: SerializeField] public float InterPersonalDistance { get; set; } = 4f;
		[field: SerializeField] public int FrameRateHz { get; private set; } = 10;
		[field: SerializeField] public float BodyInformationWeight { get; set; } = 0.5f;
		[field: SerializeField] public float FootInformationWeight { get; set; } = 0.33f;
		[field: SerializeField] public float OpponentAcceleration { get; set; } = 10f;
		[field: SerializeField] public float OpponentReactionTimeBody { get; set; } = 1f;
		[field: SerializeField] public float OpponentReactionTimeFoot { get; set; } = 0.4f;
		[field: SerializeField] public float DistanceBetweenGoals { get; set; } = 2.5f;
		[field: SerializeField, Range(0,1)] public float PassDetectionThreshold { get; set; } = 0.9f;
		public ExperimentalCondition ExperimentalCondition { get; private set; }
		public Side DominantFoot { get; set; }
		public IWebcamRecorder WebcamRecorder { get; set; } = new NotSpecifiedWebcamRecorder();
		public Opponent Opponent { get; set; }
		public Trial CurrentTrial { get; private set; }
		public LeftGoal LeftGoal { get; set; }
		public RightGoal RightGoal { get; set; }
		public int CurrentTrialIndex => _currentTrialIndex;

		public void ChangeCondition(ExperimentalCondition newCondition)
		{
			ExperimentalCondition = newCondition;
			CurrentTrial = new NoTrial();
			_currentTrialIndex = 0;
		}

		public void Bind(Side dominantFoot, LeftGoal leftGoal, RightGoal rightGoal)
		{
			DominantFoot = dominantFoot;
			LeftGoal = leftGoal;
			RightGoal = rightGoal;
			CurrentTrial = new NoTrial();
		}


		public void NextTrial()
		{
			_currentTrialIndex++;
			CurrentTrial = new Trial(_currentTrialIndex, FrameRateHz, DominantFoot, ExperimentalCondition);
		}

		int _currentTrialIndex;
	}

}
using System;
using Interactions.Domain.Goals;
using Interactions.Domain.Opponents;
using Interactions.Domain.VideoRecorder;
using Interactions.Infra;
using UnityEngine;

namespace Interactions.Domain
{
	[Serializable]
	public class Experiment
	{
		[field: SerializeReference] public float InterPersonalDistance { get; set; } = 4f;
		[field: SerializeReference] public int FrameRateHz { get; private set; } = 10;
		[field: SerializeReference] public float BodyInformationWeight { get; set; } = 0.5f;
		[field: SerializeReference] public float FootInformationWeight { get; set; } = 0.33f;
		[field: SerializeReference] public float OpponentAcceleration { get; set; } = 10f;
		[field: SerializeReference] public float OpponentReactionTimeBody { get; set; } = 1f;
		[field: SerializeReference] public float OpponentReactionTimeFoot { get; set; } = 0.4f;
		[field: SerializeReference] public float DistanceBetweenGoals { get; set; } = 2.5f;
		public PassCorrector PassCorrector { get; set; } = new PassCorrector(); 
		public Side DominantFoot { get; set; }
		public IWebcamRecorder WebcamRecorder { get; set; } = new NotInitiatedRecorder();
		public Opponent Opponent { get; set; }
		public Ball Ball { get; set; }
		public Trial CurrentTrial { get; private set; }
		public LeftGoal LeftGoal { get; set; }
		public RightGoal RightGoal { get; set; }

		public void Bind(Side dominantFoot, LeftGoal leftGoal, RightGoal rightGoal)
		{
			DominantFoot = dominantFoot;
			LeftGoal = leftGoal;
			RightGoal = rightGoal;
		}


		public void NextTrial()
		{
			_currentTrialIndex++;
			CurrentTrial = new Trial(_currentTrialIndex, FrameRateHz, DominantFoot);
		}

		int _currentTrialIndex;
	}

}
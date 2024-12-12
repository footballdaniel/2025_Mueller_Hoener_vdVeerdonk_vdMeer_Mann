using Interactions.Domain.Goals;
using Interactions.Domain.VideoRecorder;
using Interactions.Infra;

namespace Interactions.Domain
{
	public class Experiment
	{
		public Experiment(int frameRateHz, Side dominantFoot)
		{
			FrameRateHz = frameRateHz;
			DominantFoot = dominantFoot;
		}

		public float InterPersonalDistance { get; set; } = 4f;
		public Side DominantFoot { get; set; }
		public int FrameRateHz { get; private set; }
		public IWebcamRecorder WebcamRecorder { get; set; } = new NotInitiatedRecorder();
		public Opponent Opponent { get; set; }
		public Ball Ball { get; set; }
		public Trial CurrentTrial { get; private set; }
		public float OpponentAcceleration { get; set; } = 10f;
		public float OpponentReactionTime { get; set; } = 0.2f;
		public float DistanceBetweenGoals { get; set; } = 2f;
		public LeftGoal LeftGoal { get; set; }
		public RightGoal RightGoal { get; set; }

		public void NextTrial()
		{
			_currentTrialIndex++;
			CurrentTrial = new Trial(_currentTrialIndex, FrameRateHz, DominantFoot);
		}

		int _currentTrialIndex;
	}

}
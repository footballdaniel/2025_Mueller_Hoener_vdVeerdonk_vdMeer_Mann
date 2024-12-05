using Interactions.Domain.VideoRecorder;

namespace Interactions.Domain
{
	public class Experiment
	{
		public Experiment(int frameRateHz, Side dominantFoot)
		{
			FrameRateHz = frameRateHz;
			DominantFoot = dominantFoot;
		}

		public Side DominantFoot { get; set; }
		public int FrameRateHz { get; private set; }
		public IWebcamRecorder WebcamRecorder { get; set; }
		public Opponent Opponent { get; set; }
		public Ball Ball { get; set; }
		public Trial CurrentTrial { get; private set; }

		public void NextTrial()
		{
			_currentTrialIndex++;
			CurrentTrial = new Trial(_currentTrialIndex, FrameRateHz, DominantFoot);
		}

		int _currentTrialIndex;
	}
}
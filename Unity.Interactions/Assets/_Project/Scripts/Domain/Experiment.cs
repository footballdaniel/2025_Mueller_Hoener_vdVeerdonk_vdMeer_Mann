using Domain;
using Domain.VideoRecorder;

namespace App
{
	public class Experiment
	{
		public Experiment(int frameRateHz)
		{
			FrameRateHz = frameRateHz;
		}

		public int FrameRateHz { get; private set; }
		public IWebcamRecorder WebcamRecorder { get; set; }
		public Opponent Opponent { get; set; }
		public Ball Ball { get; set; }
		public Trial CurrentTrial { get; private set; }

		public void NextTrial()
		{
			_currentTrialIndex++;
			CurrentTrial = new Trial(_currentTrialIndex, FrameRateHz);
		}

		int _currentTrialIndex;
	}
}
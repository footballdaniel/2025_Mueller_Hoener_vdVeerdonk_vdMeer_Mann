using Domain;
using Domain.VideoRecorder;

namespace App
{
	public class SessionState
	{
		public IWebcamRecorder WebcamRecorder { get; set; }
		public Experiment Experiment { get; private set; } = new();
		public Trial CurrentTrial { get; set; }
		public Opponent Opponent { get; set; }
		public Ball Ball { get; set; }
	}
}
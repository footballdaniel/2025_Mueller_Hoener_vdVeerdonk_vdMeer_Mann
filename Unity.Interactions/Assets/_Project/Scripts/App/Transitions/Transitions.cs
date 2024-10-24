namespace App
{
	public class Transitions
	{
		public Transition BeginExperiment { get; set; }
		public Transition SelectWebcam { get; set; }
		public Transition BeginNextTrial { get; set; }
		public Transition InitiateRecorder { get; set; }
		public Transition ExportVideo { get; set; }
		public Transition StartLabTrialWithVideoRecording { get; set; }
		public Transition EndTrial { get; set; }
		public Transition WaitForNextTrial { get; set; }
		public Transition FinishExport { get; set; }
		public Transition StartInSituTrialWithVideoRecording { get; set; }
	}
}
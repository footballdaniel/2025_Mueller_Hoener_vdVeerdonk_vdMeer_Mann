namespace App
{
	public class Transitions
	{
		public Transition BeginExperiment { get; set; }
		public Transition SelectWebcam { get; set; }
		public Transition NextLabTrialWithoutRecording { get; set; }
		public Transition InitiateRecorder { get; set; }
		public Transition ExportVideo { get; set; }
		public Transition NextLabTrialWithVideoRecording { get; set; }
		public Transition EndLabTrial { get; set; }
		public Transition WaitForNextTrial { get; set; }
		public Transition EndTrialLab { get; set; }
		public Transition NextInSituTrialWithVideoRecording { get; set; }
		public Transition NextInSituTrialWithoutRecording { get; set; }
		public Transition ExportVideoOfInSituTrial { get; set; }
		public Transition EndInSituTrial { get; set; }
		public Transition EndTrialInSitu { get; set; }
		public Transition EndLabTrialAfterExporting { get; set; }
		public Transition EndInSituTrialAfterExporting { get; set; }
	}
}
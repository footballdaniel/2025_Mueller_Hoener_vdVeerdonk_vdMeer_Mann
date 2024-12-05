namespace Interactions.Application.Transitions
{
	public class Transitions
	{
		public Transition BeginExperiment { get; set; }
		public Transition SelectWebcam { get; set; }
		public Transition NextLabTrialWithoutRecording { get; set; }
		public Transition InitiateRecorder { get; set; }
		public Transition ExportVideoOfLabTrial { get; set; }
		public Transition NextLabTrialWithVideoRecording { get; set; }
		public Transition EndLabTrial { get; set; }
		public Transition WaitForNextTrialLab { get; set; }
		public Transition EndTrialLab { get; set; }
		public Transition NextInSituTrialWithVideoRecording { get; set; }
		public Transition NextInSituTrialWithoutRecording { get; set; }
		public Transition ExportVideoOfInSituTrial { get; set; }
		public Transition EndInSituTrial { get; set; }
		public Transition EndLabTrialAfterExporting { get; set; }
		public Transition WaitForNextTrialInSitu { get; set; }
		public Transition SelectCondition { get; set; }
		public Transition Init { get; set; }
	}
}
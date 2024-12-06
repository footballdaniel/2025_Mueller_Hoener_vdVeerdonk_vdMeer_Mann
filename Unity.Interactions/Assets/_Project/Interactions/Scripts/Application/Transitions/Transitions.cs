namespace Interactions.Application.Transitions
{
	public class Transitions
	{
		public Transition SelectWebcam { get; set; }
		public Transition InitiateRecorder { get; set; }
		public Transition ExportVideo { get; set; }
		public Transition WaitForNextTrial { get; set; }
		public Transition ExportVideoOfInSituTrial { get; set; }
		public Transition EndLabTrialAfterExporting { get; set; }
		public Transition SelectCondition { get; set; }
		public Transition StartExperiment { get; set; }
		public Transition LaboratoryTrial { get; set; }
		public Transition InSituTrial { get; set; }
	}
}
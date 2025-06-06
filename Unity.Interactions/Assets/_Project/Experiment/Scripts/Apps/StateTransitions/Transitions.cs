namespace Interactions.Apps.StateTransitions
{
	public class Transitions
	{
		public Transition SelectWebcam { get; set; }
		public Transition StartExperiment { get; set; }
		public Transition WaitForNextTrial { get; set; }
		public Transition LaboratoryTrialInteractive { get; set; }
		public Transition InSituTrial { get; set; }
		public Transition LaboratoryTrialNonInteractive { get; set; }
		public Transition LaboratoryNoOpponent { get; set; }
		public ImmediateTransition Quit { get; set; }
	}
}
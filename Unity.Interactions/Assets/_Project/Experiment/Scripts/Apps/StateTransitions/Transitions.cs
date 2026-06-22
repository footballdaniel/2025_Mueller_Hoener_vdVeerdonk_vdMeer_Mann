namespace Interactions.Apps.StateTransitions
{
	public class Transitions
	{
		public Transition SelectWebcam { get; set; }
		public Transition StartExperiment { get; set; }
		public Transition WaitForNextTrial { get; set; }
		public ImmediateTransition Quit { get; set; }
	}
}
using UnityEngine;

namespace Interactions.Apps.StateTransitions
{
	public class ImmediateTransition
	{
		public ImmediateTransition(App app)
		{
			Application.quitting += () => app.Experiment.WebcamRecorder.StopRecording();
		}
	}
}
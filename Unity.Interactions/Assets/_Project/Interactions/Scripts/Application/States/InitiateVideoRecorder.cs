namespace Interactions.Application.States
{
	internal class InitiateVideoRecorder : State
	{
		public InitiateVideoRecorder(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_app.Experiment.WebcamRecorder.StartRecording();
		}

		public override void Tick()
		{
			_app.Transitions.WaitForNextTrial.Execute();
		}
	}
}
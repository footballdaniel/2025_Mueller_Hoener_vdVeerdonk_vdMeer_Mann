namespace Interactions.Application.States
{
	internal class InitiateVideoRecorder : State
	{
		public InitiateVideoRecorder(global::Interactions.Application.App app) : base(app)
		{
		}

		public override void Enter()
		{
			_app.Experiment.WebcamRecorder.StartRecording();
		}

		public override void Tick()
		{
			if (_app.ExperimentalCondition == ExperimentalCondition.InSitu)
				_app.Transitions.NextInSituTrialWithVideoRecording.Execute();
			else
				_app.Transitions.NextLabTrialWithVideoRecording.Execute();
		}
	}
}
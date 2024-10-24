namespace App.States
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
			if (!_app.Experiment.WebcamRecorder.IsRecording)
				return;

			if (_app.ExperimentalCondition == ExperimentalCondition.InSitu)
				_app.Transitions.NextInSituTrialWithVideoRecording.Execute();
			else
				_app.Transitions.NextLabTrialWithVideoRecording.Execute();
		}
	}
}
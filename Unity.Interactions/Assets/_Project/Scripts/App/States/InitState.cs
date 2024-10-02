namespace App.States
{
	internal class InitState : State
	{

		public InitState(App app) : base(app)
		{
		}


		public override void Enter()
		{
			var presenter = new ExperimentViewModel(_app);
			_app.UI.ExperimentUI.Set(presenter);
		}

		public override void Tick()
		{
			if (_app.RecordVideo)
				_app.Transitions.SelectWebcam.Execute();
			else
				_app.Transitions.BeginExperiment.Execute();
		}
		
		
	}
}
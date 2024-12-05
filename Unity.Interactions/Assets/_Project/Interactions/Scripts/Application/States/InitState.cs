using Interactions.Application.ViewModels;

namespace Interactions.Application.States
{
	internal class InitState : State
	{

		public InitState(App app) : base(app)
		{
		}


		public override void Enter()
		{
			var experimentViewModel = new ExperimentViewModel(_app);
			_app.UI.ExperimentOverlay.Bind(experimentViewModel);

			if (_app.ExperimentalCondition == ExperimentalCondition.InSitu)
				_app.UI.InSituUI.Show();
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
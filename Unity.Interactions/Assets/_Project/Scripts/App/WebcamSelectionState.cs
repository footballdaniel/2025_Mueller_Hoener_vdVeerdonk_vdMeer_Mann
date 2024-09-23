using _Project.Scripts.App.States;

namespace _Project.Scripts.App
{
	internal class WebcamSelectionState : State
	{
		public WebcamSelectionState(App app) : base(app)
		{
		}

		public override void Enter()
		{
			var availableWebCams = new AvailableWebCams();
			var presenter = new ExperimentPresenter(availableWebCams, _app.Transitions);
			_app.UI.Set(presenter);
		}
	}
}
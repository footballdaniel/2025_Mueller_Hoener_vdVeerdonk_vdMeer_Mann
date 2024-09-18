namespace _Project.Scripts.App.States
{
	internal class InitState : State
	{

	
		public override void Enter()
		{
			var availableWebCams = new AvailableWebCams();
			var presenter = new ExperimentPresenter(availableWebCams);
			_app.UI.Set(presenter);
		}
	


		public InitState(App app) : base(app)
		{
		}


	}
}
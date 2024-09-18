namespace _Project.Scripts.App.States
{
	internal class InitState : State
	{

	
		public override void Enter()
		{
			var presenter = new ExperimentPresenter(_app.UI);
		}
	


		public InitState(App app) : base(app)
		{
		}


	}
}
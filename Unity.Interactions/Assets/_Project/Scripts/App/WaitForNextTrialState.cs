using App.States;

namespace App
{
	internal class WaitForNextTrialState : State
	{
		public WaitForNextTrialState(App app) : base(app)
		{
		}
		
		public override void Enter()
		{
			var presenter = new ExperimentPresenter(_app);
			_app.UI.ExperimentUI.Set(presenter);
		}
	}
}
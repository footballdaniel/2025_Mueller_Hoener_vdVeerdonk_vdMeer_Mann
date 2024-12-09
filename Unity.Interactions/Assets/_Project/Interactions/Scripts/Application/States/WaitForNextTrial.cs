namespace Interactions.Application.States
{
	internal class WaitForNextTrial : State
	{
		public WaitForNextTrial(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_app.UI.ExperimentOverlay.Bind(_app.ExperimentViewModel);
			_app.UI.ExperimentOverlay.Show();
		}
	}
}
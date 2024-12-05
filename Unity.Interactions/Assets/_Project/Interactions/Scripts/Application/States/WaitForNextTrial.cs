namespace Interactions.Scripts.Application.States
{
	internal class WaitForNextTrial : State
	{
		public WaitForNextTrial(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_app.UI.ExperimentOverlay.Show();
		}
	}
}
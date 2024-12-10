using Interactions.Apps.States;

namespace Interactions.Apps
{
	internal class StartupXr : State
	{
		public StartupXr(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_app.UI.XRStatusUI.Show();
			_app.UI.XRStatusUI.Bind(_app.XRStatusViewModel);
		}

		public override void Exit()
		{
			_app.UI.XRStatusUI.Hide();
		}
	}
}
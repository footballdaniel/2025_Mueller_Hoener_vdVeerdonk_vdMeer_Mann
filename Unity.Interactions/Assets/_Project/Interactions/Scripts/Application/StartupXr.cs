using Interactions.Application.States;
using Interactions.Application.ViewModels;

namespace Interactions.Application
{
	internal class StartupXr : State
	{
		public StartupXr(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_app.UI.XRStatusUI.Bind(_app.XRStatusViewModel);
			_app.UI.XRStatusUI.Show();
		}

		public override void Exit()
		{
			_app.UI.XRStatusUI.Hide();
		}
	}
}
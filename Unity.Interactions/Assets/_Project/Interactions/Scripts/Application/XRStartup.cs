using Interactions.Scripts.Application.States;
using Interactions.Scripts.Application.ViewModels;

namespace Interactions.Scripts.Application
{
	internal class XRStartup : State
	{
		public XRStartup(App app) : base(app)
		{
		}

		public override void Enter()
		{
			var xrStatusViewModel = new XRStatusViewModel();
			_app.UI.XRStatusUI.Bind(xrStatusViewModel);
			_app.UI.XRStatusUI.Show();

			if (!XRStatus.HasXRErrors())
				_app.Transitions.SelectCondition.Execute();
		}

		public override void Exit()
		{
			_app.UI.XRStatusUI.Hide();
		}
	}
}
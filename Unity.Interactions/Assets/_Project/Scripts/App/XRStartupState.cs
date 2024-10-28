using App.States;

namespace App
{
	internal class XRStartupState : State
	{
		public XRStartupState(App app) : base(app)
		{
			
		}

		public override void Enter()
		{
			var xrStatusViewModel = new XRStatusViewModel();
			_app.UI.XRStatusUI.Bind(xrStatusViewModel);
			_app.UI.XRStatusUI.Show();
			
			if (!XRStatus.HasXRErrors())
				_app.Transitions.Init.Execute();
		}
		
		public override void Exit()
		{
			_app.UI.XRStatusUI.Hide();
		}
	}
}
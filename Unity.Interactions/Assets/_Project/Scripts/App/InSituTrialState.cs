using App.States;

namespace App
{
	internal class InSituTrialState : State
	{
		public InSituTrialState(App app) : base(app)
		{
		}

		public override void Enter()
		{
			var viewModel = new InSituViewModel();
			_app.UI.InSituUI.Bind(viewModel);
		}
	}
}
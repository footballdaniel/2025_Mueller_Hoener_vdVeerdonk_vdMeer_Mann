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
			if (_app.ExperimentalCondition == ExperimentalCondition.InSitu)
				_app.UI.InSituUI.Show();
		}
		
	}
}
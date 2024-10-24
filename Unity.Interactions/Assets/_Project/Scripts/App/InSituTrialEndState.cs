using App.States;

namespace App
{
	internal class InSituTrialEndState : State
	{
		public InSituTrialEndState(App app) : base(app)
		{
		}
		
		public override void Enter()
		{
			_app.Transitions.WaitForNextTrialInSitu.Execute();
		}
	}
}
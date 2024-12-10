namespace Interactions.Apps.States
{
	public class State
	{
		protected global::Interactions.Apps.App _app;

		protected State(global::Interactions.Apps.App app)
		{
			_app = app;
		}

		public virtual void Enter()
		{
			
		}

		public virtual void Exit()
		{
		}


		public virtual void Tick()
		{
		}
	}
}
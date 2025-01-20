namespace Interactions.Apps.States
{
	public class State
	{

		protected State(App app)
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

		protected App _app;
	}
}
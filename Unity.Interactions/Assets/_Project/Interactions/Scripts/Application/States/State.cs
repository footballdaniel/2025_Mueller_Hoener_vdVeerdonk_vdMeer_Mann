namespace Interactions.Scripts.Application.States
{
	public class State
	{
		protected readonly global::Interactions.Scripts.Application.App _app;

		protected State(global::Interactions.Scripts.Application.App app)
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
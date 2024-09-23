using System.Collections.Generic;

namespace _Project.Scripts.App.States
{
	public class State
	{
		protected readonly App _app;

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
	}
}
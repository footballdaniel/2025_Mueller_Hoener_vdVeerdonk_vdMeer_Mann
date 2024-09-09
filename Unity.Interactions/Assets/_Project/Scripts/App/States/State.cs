using System.Collections.Generic;

namespace _Project.Scripts.App.States
{
	internal class State
	{
		protected readonly App _app;

		protected State(App app)
		{
			_app = app;
		}

		public List<Transition> Transitions { get; } = new();

		public void AddTransition(Transition transition)
		{
			Transitions.Add(transition);
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
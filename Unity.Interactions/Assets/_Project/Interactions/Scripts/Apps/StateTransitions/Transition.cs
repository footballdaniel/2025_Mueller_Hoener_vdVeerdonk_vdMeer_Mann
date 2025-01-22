using System;
using Interactions.Apps.States;

namespace Interactions.Apps.StateTransitions
{
	public class Transition
	{
		public Transition(App app, State from, State to)
		{
			_stateMachine = app.StateMachine;
			_from = new[] { from };
			_to = to;
		}

		public Transition(App app, State[] from, State to)
		{
			_stateMachine = app.StateMachine;
			_from = from;
			_to = to;
		}

		public void Execute()
		{
			if (Array.IndexOf(_from, _stateMachine.CurrentState) < 0)
				return;

			_stateMachine.SetState(_to);
		}

		readonly State[] _from;
		readonly StateMachine _stateMachine;
		readonly State _to;
	}
}
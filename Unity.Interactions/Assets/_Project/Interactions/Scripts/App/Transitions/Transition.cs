using _Project.Interactions.Scripts.App.States;

namespace _Project.Interactions.Scripts.App.Transitions
{
	public class Transition
	{
		public Transition(App app, State from, State to, IPredicate predicate = null)
		{
			_stateMachine = app.StateMachine;
			_from = from;
			_to = to;

			_predicate = predicate ?? new NoPredicate();
		}

		public void Execute()
		{
			if (_stateMachine.CurrentState != _from)
				return;

			if (!_predicate.IsTrue())
				return;

			_stateMachine.SetState(_to);
		}

		readonly State _from;
		readonly StateMachine _stateMachine;
		readonly State _to;
		readonly IPredicate _predicate;
	}
}
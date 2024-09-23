using UnityEngine;

namespace _Project.Scripts.App.States
{
	public class StateMachine
	{
		State _currentState;
		public State CurrentState => _currentState;

		public void SetState(State state)
		{
			_currentState?.Exit();
			_currentState = state;
			_currentState.Enter();

			Debug.Log($"State changed to {state.GetType().Name}");
		}

		public void Tick()
		{
			_currentState?.Tick();
		}
	}
}
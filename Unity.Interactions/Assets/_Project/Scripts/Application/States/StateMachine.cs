using UnityEngine;

internal class StateMachine
{
	GameState _currentState;

	public void SetState(GameState state)
	{
		_currentState?.Exit();
		_currentState = state;
		_currentState.Enter();

		Debug.Log($"State changed to {state.GetType().Name}");
	}

	public void Tick()
	{
		TryTransition();

		_currentState?.Tick();
	}

	void TryTransition()
	{
		foreach (var transition in _currentState.Transitions)
			if (transition.TryTransition(out var state))
			{
				SetState(state);
				return;
			}
	}
}
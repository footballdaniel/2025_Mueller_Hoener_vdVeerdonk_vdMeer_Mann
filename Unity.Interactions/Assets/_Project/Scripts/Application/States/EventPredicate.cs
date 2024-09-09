using System;

internal class EventPredicate : IPredicate
{
	bool _eventTriggered;

	public EventPredicate(ref Action eventTrigger)
	{
		eventTrigger += OnTriggered;
	}

	public bool ShouldTransition()
	{
		return _eventTriggered;
	}

	void OnTriggered()
	{
		_eventTriggered = true;
	}
}
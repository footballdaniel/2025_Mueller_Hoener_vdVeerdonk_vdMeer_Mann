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
		if (!_eventTriggered)
			return false;
		
		_eventTriggered = false;
		return true;
	}

	void OnTriggered()
	{
		_eventTriggered = true;
	}
}
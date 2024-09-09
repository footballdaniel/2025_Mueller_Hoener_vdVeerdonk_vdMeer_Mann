using System;
using UnityEngine;

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
		Debug.Log("Triggered");
		_eventTriggered = true;
	}
}
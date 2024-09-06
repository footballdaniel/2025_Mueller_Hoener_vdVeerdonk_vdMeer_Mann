using System;
using UnityEngine;

internal interface IPredicate
{
    bool ShouldTransition();
}

internal record BooleanPredicate(bool ShouldTransitionValue) : IPredicate
{
    public bool ShouldTransition() => ShouldTransitionValue;
}

internal class EventPredicate : IPredicate
{
    private bool _eventTriggered;

    public EventPredicate(Action eventTrigger)
    {
        Debug.Log("constructor");
        eventTrigger += OnTriggered;
    }

    private void OnTriggered()
    {
        Debug.Log("Triggered");
        _eventTriggered = true;
    }

    public bool ShouldTransition() => _eventTriggered;
}


internal record Transition(GameState To, IPredicate Predicate)
{
    public bool TryTransition(out GameState state)
    {
        state = To;
        return Predicate.ShouldTransition();
    }
}
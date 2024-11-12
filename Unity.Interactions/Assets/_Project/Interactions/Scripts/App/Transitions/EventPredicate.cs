using System;
using _Project.Interactions.Scripts.App.States;

namespace _Project.Interactions.Scripts.App.Transitions
{
	internal class EventPredicate : IPredicate
	{
		bool _eventTriggered;

		public EventPredicate(ref Action eventTrigger)
		{
			eventTrigger += OnTriggered;
		}

		public bool IsTrue()
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
}
using System;
using Interactions.Scripts.Application.States;

namespace Interactions.Scripts.Application.Transitions
{
	internal class CompositePredicate : IPredicate
	{
		bool _eventTriggered;
		readonly bool _flag;

		public CompositePredicate (bool flag, ref Action trigger)
		{
			_flag = flag;
			
			trigger += OnTriggered;
		}

		void OnTriggered()
		{
			_eventTriggered = true;
		}

		public bool IsTrue()
		{
			if (!_eventTriggered || !_flag)
				return false;
		
			_eventTriggered = false;
			return true;
		}
	}
}
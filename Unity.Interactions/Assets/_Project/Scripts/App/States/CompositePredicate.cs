using System;

namespace _Project.Scripts.App.States
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

		public bool ShouldTransition()
		{
			if (!_eventTriggered || !_flag)
				return false;
		
			_eventTriggered = false;
			return true;
		}
	}
}
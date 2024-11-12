using System;
using _Project.Interactions.Scripts.App.States;

namespace _Project.Interactions.Scripts.App.Transitions
{
	internal class BooleanPredicate : IPredicate
	{
		readonly Func<bool> _flag;

		public BooleanPredicate(Func<bool> flag)
		{
			_flag = flag;
		}

		public bool IsTrue()
		{
			return _flag();
		}
	}
}
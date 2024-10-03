using System;

namespace App.States
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
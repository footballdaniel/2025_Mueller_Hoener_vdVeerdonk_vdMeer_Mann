namespace _Project.Scripts.App.States
{
	internal class BooleanPredicate : IPredicate
	{
		readonly bool _flag;

		public BooleanPredicate(bool flag)
		{
			_flag = flag;
		}

		public bool ShouldTransition()
		{
			return _flag;
		}
	}
}
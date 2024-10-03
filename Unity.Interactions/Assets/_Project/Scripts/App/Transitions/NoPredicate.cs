using App.States;

namespace App
{
	public class NoPredicate : IPredicate
	{
		public bool IsTrue()
		{
			return true;
		}
	}
}
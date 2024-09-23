using _Project.Scripts.App.States;

namespace _Project.Scripts.App
{
	public class NoPredicate : IPredicate
	{
		public bool IsTrue()
		{
			return true;
		}
	}
}
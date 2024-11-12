using _Project.Interactions.Scripts.App.States;

namespace _Project.Interactions.Scripts.App.Transitions
{
	public class NoPredicate : IPredicate
	{
		public bool IsTrue()
		{
			return true;
		}
	}
}
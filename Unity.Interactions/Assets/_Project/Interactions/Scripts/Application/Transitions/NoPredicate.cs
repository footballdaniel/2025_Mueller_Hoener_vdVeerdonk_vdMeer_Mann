using Interactions.Scripts.Application.States;

namespace Interactions.Scripts.Application.Transitions
{
	public class NoPredicate : IPredicate
	{
		public bool IsTrue()
		{
			return true;
		}
	}
}
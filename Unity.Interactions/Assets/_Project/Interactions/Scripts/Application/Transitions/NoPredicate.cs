using Interactions.Application.States;

namespace Interactions.Application.Transitions
{
	public class NoPredicate : IPredicate
	{
		public bool IsTrue()
		{
			return true;
		}
	}
}
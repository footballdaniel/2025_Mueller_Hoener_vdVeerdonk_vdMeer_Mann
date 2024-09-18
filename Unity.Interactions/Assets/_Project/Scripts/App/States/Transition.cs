namespace _Project.Scripts.App.States
{
	internal record Transition(State To, IPredicate Predicate)
	{
		public bool TryTransition(out State state)
		{
			state = To;
			return Predicate.ShouldTransition();
		}
	}
}
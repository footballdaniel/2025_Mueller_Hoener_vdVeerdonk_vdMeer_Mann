internal record Transition(GameState To, IPredicate Predicate)
{
	public bool TryTransition(out GameState state)
	{
		state = To;
		return Predicate.ShouldTransition();
	}
}
internal record BooleanPredicate(bool ShouldTransitionValue) : IPredicate
{
	public bool ShouldTransition()
	{
		return ShouldTransitionValue;
	}
}
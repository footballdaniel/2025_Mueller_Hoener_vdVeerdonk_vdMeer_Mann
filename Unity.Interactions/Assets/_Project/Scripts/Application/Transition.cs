internal record Transition(GameState To, bool ShouldTransition)
{
    public bool TryTransition(out GameState state)
    {
        state = To;
        return ShouldTransition;
    }
}
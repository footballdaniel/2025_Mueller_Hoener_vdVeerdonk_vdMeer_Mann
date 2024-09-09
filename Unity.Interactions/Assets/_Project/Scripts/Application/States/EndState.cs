using UnityEngine;

internal class EndState : GameState
{
    public EndState(App app) : base(app)
    {
    }

    public override void Enter()
    {
        Object.Destroy(_context.Opponent.gameObject);
    }
}
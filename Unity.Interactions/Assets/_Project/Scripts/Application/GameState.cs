using System.Collections.Generic;

internal class GameState
{
    public void AddTransition(Transition transition)
    {
        Transitions.Add(transition);
    }
	
    protected readonly Game _context;
    public List<Transition> Transitions { get; } = new List<Transition>();

    protected GameState(Game context)
    {
        _context = context;
    }


    public virtual void Enter()
    {
    }
	
    public virtual void Exit()
    {
    }


    public virtual void Tick()
    {
    }
}
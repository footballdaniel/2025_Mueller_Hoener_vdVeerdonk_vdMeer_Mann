using UnityEngine;

internal class TrialState : GameState
{
    public TrialState(App app) : base(app)
    {
    }

    public override void Enter()
    {
        _context.Trial = new Trial(Time.timeSinceLevelLoad);
        _context.Opponent = Object.Instantiate(_context.OpponentPrefab);
        _context.Opponent.Set(_context.User);
    }

    public override void Tick()
    {
        _context.Trial.Tick(Time.deltaTime);

        if (_context.Trial.Duration > 10f)
        {
            if (!_context.RecordVideo)
            {
                AppEvents.TrialEnded?.Invoke();
                return;
            }
            
            _context.WebcamRecorder.StopRecording();
            
            if (!_context.WebcamRecorder.IsRecording)
            {
                AppEvents.TrialEnded?.Invoke();
            }
        }
    }

    public override void Exit()
    {
    }
}
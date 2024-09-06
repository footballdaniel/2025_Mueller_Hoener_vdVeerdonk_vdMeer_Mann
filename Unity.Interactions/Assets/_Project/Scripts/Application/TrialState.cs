using UnityEngine;

internal class TrialState : GameState
{
    public TrialState(Game game) : base(game)
    {
    }

    public override void Tick()
    {

        if (Time.timeSinceLevelLoad > 3)
        {
            _context.WebcamRecorder.StopRecording();
            
            if (!_context.WebcamRecorder.IsRecording)
            {
                GameEvents.TrialEnded?.Invoke();
            }
        }



    }

    public override void Exit()
    {
    }
}
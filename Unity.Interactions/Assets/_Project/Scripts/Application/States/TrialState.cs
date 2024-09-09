using UnityEngine;

internal class TrialState : GameState
{
    public TrialState(App app) : base(app)
    {
    }

    public override void Tick()
    {

        if (Time.timeSinceLevelLoad > 3)
        {
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
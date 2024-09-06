using UnityEngine;

internal class InitState : GameState
{

	
    public override void Enter()
    {
        SetRecordingFrameRate(_context.WebcamRecorder.FrameRate);
    }
	
    private void SetRecordingFrameRate(float recordingFrameRateHz)
    {
        Time.fixedDeltaTime = 1f / recordingFrameRateHz;
    }

    public InitState(Game context) : base(context)
    {
    }


}
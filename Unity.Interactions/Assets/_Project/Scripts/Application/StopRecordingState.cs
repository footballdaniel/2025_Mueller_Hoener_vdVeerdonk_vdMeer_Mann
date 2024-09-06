internal class StopRecordingState : GameState
{
    public StopRecordingState(Game game) : base(game)
    {
    }

    public override void Tick()
    {
        if (!_context.WebcamRecorder.IsRecording)
            GameEvents.RecordingStopped?.Invoke();
    }

    public override void Enter()
    {
        _context.WebcamRecorder.StopRecording();
    }
}
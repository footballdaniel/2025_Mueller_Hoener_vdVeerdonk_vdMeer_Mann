internal class StartRecordingState : GameState
{
    public StartRecordingState(Game game) : base(game)
    {
    }

    public override void Enter()
    {
        _context.WebcamRecorder.StartRecording();
    }
    
    public override void Tick()
    {
        if (_context.WebcamRecorder.IsRecording)
            GameEvents.RecordingStarted?.Invoke();
    }
}
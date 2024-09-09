internal class StartRecordingVideoState : GameState
{
    public StartRecordingVideoState(App app) : base(app)
    {
    }

    public override void Enter()
    {
        _context.WebcamRecorder.InitiateRecorder();

    }

    public override void Tick()
    {
        if (_context.WebcamRecorder.IsRecording)
            AppEvents.RecordingStarted?.Invoke();
    }
}
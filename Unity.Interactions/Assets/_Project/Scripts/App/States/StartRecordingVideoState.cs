namespace _Project.Scripts.App.States
{
    internal class StartRecordingVideoState : State
    {
        public StartRecordingVideoState(App app) : base(app)
        {
        }

        public override void Enter()
        {
            _app.WebcamRecorder.InitiateRecorder();

        }

        public override void Tick()
        {
            if (_app.WebcamRecorder.IsRecording)
                _app.Transitions.StartTrial.Execute();
        }
    }
}
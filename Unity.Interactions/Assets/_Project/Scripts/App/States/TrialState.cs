using UnityEngine;

namespace _Project.Scripts.App.States
{
    internal class TrialState : State
    {
        public TrialState(App app) : base(app)
        {
        }

        public override void Enter()
        {
            _app.CurrentTrial = _app.Experiment.NextTrial();
            _app.Opponent = Object.Instantiate(_app.OpponentPrefab);
            _app.Opponent.Set(_app.User);
        }

        public override void Tick()
        {
            _app.CurrentTrial.Tick(Time.deltaTime);

            if (_app.CurrentTrial.Duration > 10f)
            {
                if (!_app.RecordVideo)
                {
                    Events.TrialEnded?.Invoke();
                    return;
                }
            
                _app.WebcamRecorder.StopRecording();
            
                if (!_app.WebcamRecorder.IsRecording)
                {
                    Events.TrialEnded?.Invoke();
                }
            }
        }

        public override void Exit()
        {
        }
    }
}
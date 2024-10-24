using UnityEngine;

namespace App.States
{
    internal class LabTrialEndState : State
    {
        public LabTrialEndState(App app) : base(app)
        {
        }

        public override void Enter()
        {
            Object.Destroy(_app.Experiment.Ball.gameObject);
            Object.Destroy(_app.Experiment.Opponent.gameObject);
        }

        public override void Tick()
        {
            _app.Transitions.WaitForNextTrialLab.Execute();
        }
    }
}
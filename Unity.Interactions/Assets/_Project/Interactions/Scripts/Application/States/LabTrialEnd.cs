using UnityEngine;

namespace Interactions.Application.States
{
    internal class LabTrialEnd : State
    {
        public LabTrialEnd(global::Interactions.Application.App app) : base(app)
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
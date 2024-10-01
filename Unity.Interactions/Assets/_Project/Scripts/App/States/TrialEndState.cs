using UnityEngine;

namespace App.States
{
    internal class TrialEndState : State
    {
        public TrialEndState(App app) : base(app)
        {
        }

        public override void Enter()
        {
            Object.Destroy(_app.Ball?.gameObject);
            Object.Destroy(_app.Opponent.gameObject);
        }
    }
}
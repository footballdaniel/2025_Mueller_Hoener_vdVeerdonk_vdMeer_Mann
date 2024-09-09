using UnityEngine;

namespace _Project.Scripts.App.States
{
    internal class EndState : State
    {
        public EndState(App app) : base(app)
        {
        }

        public override void Enter()
        {
            Object.Destroy(_app.Opponent.gameObject);
        }
    }
}
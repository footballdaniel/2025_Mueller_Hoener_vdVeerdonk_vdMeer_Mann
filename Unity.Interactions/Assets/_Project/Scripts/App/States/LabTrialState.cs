using Domain;
using UnityEngine;

namespace App.States
{
	internal class LabTrialState : State
	{
		public LabTrialState(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_app.TrialState.CurrentTrial = _app.TrialState.Experiment.NextTrial();
			_app.TrialState.Opponent = Object.Instantiate(_app.OpponentPrefab);
			_app.TrialState.Ball = Object.Instantiate(_app.BallPrefab);
			_app.TrialState.Opponent.Set(_app.User);


			_app.User.DominantFoot.Passed += OnPassed;
		}

		public override void Exit()
		{
			_hasPassed = false;
			_app.User.DominantFoot.Passed -= OnPassed;
		}

		public override void Tick()
		{
			_app.TrialState.CurrentTrial.Tick(Time.deltaTime);

			if (!(_app.TrialState.CurrentTrial.Duration > 10f))
				return;

			switch (_app.ExperimentalCondition)
			{
				case ExperimentalCondition.Laboratory:
					_app.Transitions.EndLabTrial.Execute();
					break;
				case ExperimentalCondition.InSitu:
					_app.Transitions.EndInSituTrial.Execute();
					break;
			}
		}

		void OnPassed(Pass pass)
		{
			if (_hasPassed) return;

			_hasPassed = true;
			_app.TrialState.Ball = Object.Instantiate(_app.BallPrefab, pass.Position, Quaternion.identity);
			_app.TrialState.Ball.Set(pass);
		}

		bool _hasPassed;
	}
}
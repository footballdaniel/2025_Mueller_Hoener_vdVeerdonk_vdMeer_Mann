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
			_app.Experiment.NextTrial();
			_app.Experiment.Opponent = Object.Instantiate(_app.OpponentPrefab);
			_app.Experiment.Ball = Object.Instantiate(_app.BallPrefab);
			_app.Experiment.Opponent.Set(_app.User);


			_app.User.DominantFoot.Passed += OnPassed;
		}

		public override void Exit()
		{
			_hasPassed = false;
			_app.User.DominantFoot.Passed -= OnPassed;
		}

		public override void Tick()
		{
			var frameRateHz = 10f;
			var deltaTime = 1f / frameRateHz;
			_updateTimer += Time.fixedDeltaTime;
			var epsilon = 0.0001f;

			if (_updateTimer >= deltaTime - epsilon)
			{
				_updateTimer -= deltaTime;
				_app.Experiment.WebcamRecorder.Tick();
				_app.Experiment.CurrentTrial.Tick(Time.deltaTime);
			}

			if (!(_app.Experiment.CurrentTrial.Duration > 10f))
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
			_app.Experiment.Ball = Object.Instantiate(_app.BallPrefab, pass.Position, Quaternion.identity);
			_app.Experiment.Ball.Set(pass);
		}

		bool _hasPassed;
		float _updateTimer;
	}
}
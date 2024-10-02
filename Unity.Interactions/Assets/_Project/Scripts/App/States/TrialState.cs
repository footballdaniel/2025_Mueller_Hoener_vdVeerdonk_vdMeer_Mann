using Domain;
using UnityEngine;

namespace App.States
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
			_app.Ball = Object.Instantiate(_app.BallPrefab);
			_app.Opponent.Set(_app.User);


			_app.DominantFoot.Passed += OnPassed;
		}

		public override void Exit()
		{
			_hasPassed = false;
			_app.DominantFoot.Passed -= OnPassed;
		}

		public override void Tick()
		{
			_app.CurrentTrial.Tick(Time.deltaTime);
			
			if (!(_app.CurrentTrial.Duration > 10f))
				return;

			if (!_app.RecordVideo)
				_app.Transitions.EndTrial.Execute();
			else
				_app.Transitions.ExportVideo.Execute();
		}

		void OnPassed(Pass pass)
		{
			if (_hasPassed) return;

			_hasPassed = true;
			_app.Ball = Object.Instantiate(_app.BallPrefab, pass.Position, Quaternion.identity);
			_app.Ball.Set(pass);
		}

		bool _hasPassed;
	}
}
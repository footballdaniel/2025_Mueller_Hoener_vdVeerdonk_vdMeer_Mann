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
			_app.Session.CurrentTrial = _app.Session.Experiment.NextTrial();
			_app.Session.Opponent = Object.Instantiate(_app.OpponentPrefab);
			_app.Session.Ball = Object.Instantiate(_app.BallPrefab);
			_app.Session.Opponent.Set(_app.User, _app.Session.Teammates);


			_app.DominantFoot.Passed += OnPassed;
		}

		public override void Exit()
		{
			_hasPassed = false;
			_app.DominantFoot.Passed -= OnPassed;
		}

		public override void Tick()
		{
			_app.Session.CurrentTrial.Tick(Time.deltaTime);
			
			if (!(_app.Session.CurrentTrial.Duration > 10f))
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
			_app.Session.Ball = Object.Instantiate(_app.BallPrefab, pass.Position, Quaternion.identity);
			_app.Session.Ball.Set(pass);
		}

		bool _hasPassed;
	}
}
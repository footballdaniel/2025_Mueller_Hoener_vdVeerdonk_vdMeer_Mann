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

			switch ((_app.RecordVideo, _app.ExperimentalCondition))
			{
				case (false, _):
					_app.Transitions.EndTrial.Execute();
					break;
				case (true, ExperimentalCondition.InSitu):
					_app.Transitions.NextInSituTrialWithVideoRecording.Execute();
					break;
				case (true, _):
					_app.Transitions.ExportVideoOfLabTrial.Execute();
					break;
			}

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
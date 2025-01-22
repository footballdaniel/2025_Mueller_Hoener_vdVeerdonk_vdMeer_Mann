using Interactions.Apps.States;
using Interactions.Domain.Opponents;
using UnityEngine;

namespace Interactions.Apps
{
	internal class InSituTrial : State
	{
		float _updateTimer;
		InSituOpponent _opponent;

		public InSituTrial(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_app.Experiment.NextTrial();
			_app.Experiment.WebcamRecorder.StartRecording(_app.Experiment.CurrentTrial.TrialNumber);
			_opponent = Object.Instantiate(_app.InSituOpponentPrefab);
			_opponent.Bind(_app.Trackers.DefenderHipsTracker);
		}

		public override void Exit()
		{
			Object.Destroy(_opponent.gameObject);
			_app.Experiment.WebcamRecorder.StopRecording();
			_app.Experiment.CurrentTrial.Save();
		}

		public override void Tick()
		{
			var deltaTime = 1f / _app.Experiment.FrameRateHz;
			_updateTimer += Time.deltaTime;
			var epsilon = 0.0001f;

			if (_updateTimer >= deltaTime - epsilon)
			{
				_app.Experiment.CurrentTrial.OpponentHipPositions.Add(_opponent.Hips);
				_app.Experiment.CurrentTrial.UserHipPositions.Add(_app.User.Hips.Position);
				_app.Experiment.CurrentTrial.UserHeadPositions.Add(_app.User.Head.transform.position);
				_app.Experiment.CurrentTrial.UserDominantFootPositions.Add(_app.User.DominantFoot.transform.position);
				_app.Experiment.CurrentTrial.UserNonDominantFootPositions.Add(_app.User.NonDominantFoot.transform.position);

				_app.Experiment.WebcamRecorder.Tick();
				_app.Experiment.CurrentTrial.Tick(deltaTime);

				_updateTimer -= deltaTime;
			}
		}
	}
}
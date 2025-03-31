using Interactions.Domain;
using UnityEngine;

namespace Interactions.Apps.States
{
	internal class LaboratoryTrialNoOpponent : State
	{

		public LaboratoryTrialNoOpponent(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_app.Experiment.NextTrial();
			_app.Experiment.WebcamRecorder.StartRecording(_app.Experiment.CurrentTrial.TrialNumber, _app.Experiment.ExperimentalCondition);
		}

		public override void Exit()
		{
			_app.PassDetector.DespawnBall();
			_app.Experiment.CurrentTrial.Save();
			_app.Experiment.WebcamRecorder.StopRecording();
			_app.UI.OpponentSettingsUI.Hide();
		}

		public override void Tick()
		{
			var frameRateHz = 10f;
			var deltaTime = 1f / frameRateHz;
			_updateTimer += Time.deltaTime;
			var epsilon = 0.0001f;

			if (_updateTimer >= deltaTime - epsilon)
			{
				_app.Experiment.WebcamRecorder.Tick();
				_app.Experiment.CurrentTrial.Tick(deltaTime);

				_app.Experiment.CurrentTrial.UserHeadPositions.Add(_app.User.TrackedHead.transform.position);
				_app.Experiment.CurrentTrial.UserHipPositions.Add(_app.User.Hips.Position);
				_app.Experiment.CurrentTrial.UserDominantFootPositions.Add(_app.User.DominantFoot.transform.position);
				_app.Experiment.CurrentTrial.UserNonDominantFootPositions.Add(_app.User.NonDominantFoot.transform.position);

				if (_app.PassDetector.DetectPass())
					_app.Experiment.CurrentTrial.BallEvents.Add(new BallEvent("Pass", _app.Experiment.CurrentTrial.Timestamps[^1], _app.User.DominantFoot.transform.position));

				_updateTimer -= deltaTime;
			}
		}

		float _updateTimer;
	}
}
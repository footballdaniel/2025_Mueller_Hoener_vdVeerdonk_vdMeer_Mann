using Interactions.Domain;
using Interactions.Infra;
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
			_inputDataQueue = new InputDataQueue();
			_app.Experiment.NextTrial();
			_app.Experiment.WebcamRecorder.StartRecording(_app.Experiment.CurrentTrial.TrialNumber);
			_lastPassTime = Time.time;
		}

		public override void Exit()
		{
			_hasPassed = false;
			if (_ball)
				Object.Destroy(_ball.gameObject);

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

				_app.Experiment.CurrentTrial.UserHeadPositions.Add(_app.User.Head.transform.position);
				_app.Experiment.CurrentTrial.UserHipPositions.Add(_app.User.Hips.Position);
				_app.Experiment.CurrentTrial.UserDominantFootPositions.Add(_app.User.DominantFoot.transform.position);
				_app.Experiment.CurrentTrial.UserNonDominantFootPositions.Add(_app.User.NonDominantFoot.transform.position);
				_inputDataQueue.EnQueue(_app.User.DominantFoot.transform.position, _app.User.NonDominantFoot.transform.position, _app.Experiment.CurrentTrial.Duration);

				var prediction = _app.LstmModel.Evaluate(_inputDataQueue.ToInputData());

				if (prediction > 0.95f && Time.time - _lastPassTime >= 1f)
				{
					var passVelocity = _inputDataQueue.CalculateGetHighestObservedVelocity();
					var passDirection = new Vector3(passVelocity.normalized.x, passVelocity.normalized.y, passVelocity.normalized.z);

					var forwardDirection = Vector3.right;
					var angle = Vector3.Angle(forwardDirection, passDirection);
					if (angle > 45)
					{
						Debug.LogWarning("Pass at large angle detected, skip");
						return;
					}
					
					if (_ball)
						Object.Destroy(_ball.gameObject);

					AudioSource.PlayClipAtPoint(_app.PassSoundClip, _app.User.DominantFoot.transform.position);
					_lastPassTime = Time.time;


					var pass = new Pass(passVelocity.magnitude, _app.User.DominantFoot.transform.position, passDirection);
					pass = _app.PassCorrector.Correct(pass, Vector3.zero);

					_ball = Object.Instantiate(_app.BallPrefab, pass.Position, Quaternion.identity);
					_ball.Play(pass);

					// NOT HERE!
					// _app.Experiment.Opponent.Intercept(_ball);
					_app.Experiment.Ball = _ball;
				}

				_updateTimer -= deltaTime;
			}
		}

		Ball _ball;
		bool _hasPassed;
		InputDataQueue _inputDataQueue;
		float _lastPassTime;
		float _updateTimer;
	}
}
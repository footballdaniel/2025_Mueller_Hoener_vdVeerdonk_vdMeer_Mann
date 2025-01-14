using Interactions.Domain;
using Interactions.Infra;
using UnityEngine;

namespace Interactions.Apps.States
{
	internal class LaboratoryTrialInteractive : State
	{

		public LaboratoryTrialInteractive(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_inputDataQueue = new InputDataQueue();
			_app.Experiment.NextTrial();
			_app.Experiment.WebcamRecorder.StartRecording(_app.Experiment.CurrentTrial.TrialNumber);
			
			_app.Experiment.Opponent = Object.Instantiate(_app.OpponentPrefab);
			_app.Experiment.Opponent.Bind(_app.User, _app.LeftGoal, _app.RightGoal, true);
			_app.Experiment.Opponent.transform.Rotate(0, -90, 0);
			
			_app.UI.SettingsUI.Bind(_app.OpponentViewModel);
			_app.UI.SettingsUI.Show();

			_app.Experiment.Opponent.BallIntercepted += OnBallIntercepted;

			_lastPassTime = Time.time;
		}

		void OnBallIntercepted(Vector2 direction)
		{
			_ball.Play(new Pass(3, _ball.transform.position, new Vector3(direction.x, 0, direction.y)));
		}

		public override void Exit()
		{
			_hasPassed = false;
			Object.Destroy(_app.Experiment.Ball?.gameObject);
			Object.Destroy(_app.Experiment.Opponent.gameObject);
			
			_app.Experiment.Opponent.BallIntercepted -= OnBallIntercepted;
			_app.Experiment.CurrentTrial.Save();
			
			_app.Experiment.WebcamRecorder.StopRecording();
			
			_app.UI.SettingsUI.Hide();
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
				
				_app.Experiment.CurrentTrial.OpponentHipPositions.Add(_app.Experiment.Opponent.transform.position);
				_app.Experiment.CurrentTrial.UserHeadPositions.Add(_app.User.Head.transform.position);
				_app.Experiment.CurrentTrial.UserDominantFootPositions.Add(_app.User.DominantFoot.transform.position);
				_app.Experiment.CurrentTrial.UserNonDominantFootPositions.Add(_app.User.NonDominantFoot.transform.position);
				_inputDataQueue.EnQueue(_app.User.DominantFoot.transform.position, _app.User.NonDominantFoot.transform.position, _app.Experiment.CurrentTrial.Duration);

				var prediction = _app.LstmModel.Evaluate(_inputDataQueue.ToInputData());

				if (prediction > 0.95f && Time.time - _lastPassTime >= 1f)
				{
					if (_ball)
						Object.Destroy(_ball.gameObject);
					
					AudioSource.PlayClipAtPoint(_app.PassSoundClip, _app.User.DominantFoot.transform.position);
					_lastPassTime = Time.time;
					
					var passVelocity = _inputDataQueue.CalculateGetHighestObservedVelocity();
					var passDirection = new Vector3(passVelocity.normalized.x, passVelocity.normalized.y, passVelocity.normalized.z);
					
					var pass = new Pass(passVelocity.magnitude, _app.User.DominantFoot.transform.position, passDirection);
					pass = _app.PassCorrector.Correct(pass, _app.Experiment.Opponent.transform.position);
					_ball = Object.Instantiate(_app.BallPrefab, pass.Position, Quaternion.identity);
					_ball.Play(pass);

					_app.Experiment.Opponent.Intercept(_ball);
					_app.Experiment.Ball = _ball;
				}

				_updateTimer -= deltaTime;
			}
		}

		bool _hasPassed;
		InputDataQueue _inputDataQueue;
		float _lastPassTime;
		float _updateTimer;
		Ball _ball;
	}
}
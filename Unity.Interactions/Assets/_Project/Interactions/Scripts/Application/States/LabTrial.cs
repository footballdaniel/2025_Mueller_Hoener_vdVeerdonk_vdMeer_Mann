using Interactions.Domain;
using Interactions.Infra;
using UnityEngine;

namespace Interactions.Application.States
{
	internal class LabTrial : State
	{
		bool _hasPassed;
		InputDataQueue _inputDataQueue;
		float _updateTimer;
		float _lastPassTime;

		public LabTrial(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_inputDataQueue = new InputDataQueue();
			_app.Experiment.NextTrial();
			_app.Experiment.Opponent = Object.Instantiate(_app.OpponentPrefab);
			
			_app.Experiment.Opponent.Set(_app.User);

			// _app.User.DominantFoot.Passed += OnPassed;
			
			_lastPassTime = Time.time;
		}

		public override void Exit()
		{
			_hasPassed = false;
			// _app.User.DominantFoot.Passed -= OnPassed;
		}

		public override void Tick()
		{
			var frameRateHz = 10f;
			var deltaTime = 1f / frameRateHz;
			_updateTimer += Time.fixedDeltaTime;
			var epsilon = 0.0001f;
			
			_app.Experiment.CurrentTrial.Tick(Time.deltaTime);
			if (_updateTimer >= deltaTime - epsilon)
			{
				_app.Experiment.CurrentTrial.OpponentHipPositions.Add(_app.Experiment.Opponent.transform.position);
				_app.Experiment.CurrentTrial.UserHeadPositions.Add(_app.User.Head.transform.position);
				_app.Experiment.CurrentTrial.UserDominantFootPositions.Add(_app.User.DominantFoot.transform.position);
				_app.Experiment.CurrentTrial.UserNonDominantFootPositions.Add(_app.User.NonDominantFoot.transform.position);
				_inputDataQueue.EnQueue(_app.User.DominantFoot.transform.position, _app.User.NonDominantFoot.transform.position, _app.Experiment.CurrentTrial.Duration);
				
				var prediction = _app.LstmModel.Evaluate(_inputDataQueue.ToInputData());

				if (prediction > 0.95f && Time.time - _lastPassTime >= 1f)
				{
					
					AudioSource.PlayClipAtPoint(_app.PassSoundClip, _app.User.DominantFoot.transform.position);
					_lastPassTime = Time.time; // Update the last pass time

					var passVelocity = _inputDataQueue.CalculateGetHighestObservedVelocity();
					var passDirection = new Vector3(passVelocity.normalized.x, 0, passVelocity.normalized.z);
					var pass = new Pass(passVelocity.magnitude, _app.User.DominantFoot.transform.position, passDirection);
					var ball = Object.Instantiate(_app.BallPrefab, pass.Position, Quaternion.identity);
					ball.Play(pass);
			
					_app.Experiment.Ball = ball;
				}
				
				_updateTimer -= deltaTime;
			}
		}
	}
}
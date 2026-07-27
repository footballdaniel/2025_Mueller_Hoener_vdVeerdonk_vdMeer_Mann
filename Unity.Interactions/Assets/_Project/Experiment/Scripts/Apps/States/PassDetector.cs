using Interactions.Domain;
using Interactions.Infra;
using UnityEngine;

namespace Interactions.Apps.States
{
	public class PassDetector
	{

		public PassDetector(App app)
		{
			_app = app;
			_inputDataQueue = new InputDataQueue();
			_lastPassTime = Time.time;
		}

		public void DespawnBall()
		{
			if (_app.Ball)
				Object.Destroy(_app.Ball.gameObject);
		}

		public bool DetectPass()
		{
			_inputDataQueue.EnQueue(_app.User.DominantFoot.transform.position, _app.User.NonDominantFoot.transform.position, _app.Experiment.CurrentTrial.Duration);

			var prediction = _app.LstmModel.Evaluate(_inputDataQueue.ToInputData());

			if (prediction > _app.Experiment.PassDetectionThreshold && Time.time - _lastPassTime >= 1f)
			{
				var kickingFoot = _inputDataQueue.CalculateHighestObservedVelocity();
				var passVelocity = kickingFoot.Velocity;
				var passDirection = passVelocity.normalized;

				var forwardDirection = Vector3.right;
				var angle = Vector3.Angle(forwardDirection, passDirection);

				if (angle > 45)
				{
					Debug.LogWarning("Pass at large angle detected, skip");
					return false;
				}


				if (_app.Ball)
					Object.Destroy(_app.Ball.gameObject);

				var kickingFootPosition = kickingFoot.IsDominantFoot
					? _app.User.DominantFoot.transform.position
					: _app.User.NonDominantFoot.transform.position;

				AudioSource.PlayClipAtPoint(_app.PassSoundClip, kickingFootPosition);
				_lastPassTime = Time.time;

				var pass = new Pass(passVelocity.magnitude, kickingFootPosition, passDirection);
				pass = _app.PassCorrector.Correct(pass, Vector3.zero);

				_app.Ball = Object.Instantiate(_app.BallPrefab, pass.Position, Quaternion.identity);
				_app.Ball.Play(pass);

				return true;
			}

			return false;
		}

		readonly App _app;
		readonly InputDataQueue _inputDataQueue;
		float _lastPassTime;
	}
}
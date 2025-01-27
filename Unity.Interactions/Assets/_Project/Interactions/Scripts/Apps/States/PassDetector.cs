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
				var passVelocity = _inputDataQueue.CalculateGetHighestObservedVelocity();
				var passDirection = new Vector3(passVelocity.normalized.x, passVelocity.normalized.y, passVelocity.normalized.z);

				var forwardDirection = Vector3.right;
				var angle = Vector3.Angle(forwardDirection, passDirection);

				if (angle > 45)
				{
					Debug.LogWarning("Pass at large angle detected, skip");
					return false;
				}


				if (_app.Ball)
					Object.Destroy(_app.Ball.gameObject);

				AudioSource.PlayClipAtPoint(_app.PassSoundClip, _app.User.DominantFoot.transform.position);
				_lastPassTime = Time.time;
				
				var pass = new Pass(passVelocity.magnitude, _app.User.DominantFoot.transform.position, passDirection);
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
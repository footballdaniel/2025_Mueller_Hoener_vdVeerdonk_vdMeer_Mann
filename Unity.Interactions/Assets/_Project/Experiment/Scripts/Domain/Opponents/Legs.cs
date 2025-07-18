using System;
using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class Legs : MonoBehaviour
	{
		[SerializeField] Ball _ball;
		[SerializeField] Animator _animator;
		[SerializeField] Transform _rightFoot;
		[SerializeField] Transform _leftFoot;
		[SerializeField] float _startKickDistance = 1.5f;
		[SerializeField] User _attacker;

		public event Action<Vector3> BallIntercepted;

		void Update()
		{
			if (!_ball)
				return;


			var rightFootVelocity = (_rightFootPositionLastFrame - _rightFoot.position) / Time.deltaTime;
			var leftFootVelocity = (_leftFootPositionLastFrame - _leftFoot.position) / Time.deltaTime;

			_rightFootPositionLastFrame = _rightFoot.position;
			_leftFootPositionLastFrame = _leftFoot.position;

			var distanceBallToRightFoot = Vector3.Distance(_ball.transform.position, _rightFoot.position);
			var distanceBallToLeftFoot = Vector3.Distance(_ball.transform.position, _leftFoot.position);
			var distanceBallToCloserFoot = Mathf.Min(distanceBallToRightFoot, distanceBallToLeftFoot);
			
			_isInterceptingWithRightFoot = _attacker.Position.y > 0;

			var distancePlayerToBall = Vector3.Distance(transform.position, _ball.transform.position);

			// lerp between start kick distance (0) and distance (1)
			var kickProgressPercentage = 1 - Mathf.Clamp01(distancePlayerToBall / _startKickDistance);

			var timeSinceLastKick = Time.time - _lastKickTime;

			if (timeSinceLastKick < 1f)
				kickProgressPercentage = 0;


			if (distanceBallToCloserFoot < 0.2f)
			{
				_hasKicked = true;
				_lastKickTime = Time.time;

				var kickingFootVelocity = _isInterceptingWithRightFoot ? rightFootVelocity : leftFootVelocity;
				var clampedRightFootVelocity = Vector3.ClampMagnitude(kickingFootVelocity, 3f);
				BallIntercepted?.Invoke(clampedRightFootVelocity);
				_opponent.FinishedKicking();
				_ball = null;
			}

			if (_hasKicked)
			{
				// lerp weights back
				_ikWeight = Mathf.Lerp(1, 0, (Time.time - _lastKickTime) / 0.5f);

				if (_ikWeight < 0.01f)
					_hasKicked = false;
			}

			_ikWeight = kickProgressPercentage;
		}

		public void Bind(Opponent opponent, User attacker)
		{
			_opponent = opponent;
			_attacker = attacker;
		}

		public void StartKickingTheBall(Ball ball, float startKickDistance)
		{
			_hasKicked = false;
			_startKickDistance = startKickDistance;
			_ball = ball;
		}

		void OnAnimatorIK(int layerIndex)
		{
			if (!_ball)
				return;

			var ballPosition = _ball.transform.position;
			var localBallPosition = transform.InverseTransformPoint(ballPosition);
			var constrainedPosition = new Vector3(localBallPosition.x, localBallPosition.y, 0); // Use only left/right (X) and up/down (Y)
			var worldConstrainedPosition = transform.TransformPoint(constrainedPosition);

			if (_isInterceptingWithRightFoot)
			{
				_animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _ikWeight);
				_animator.SetIKPosition(AvatarIKGoal.RightFoot, worldConstrainedPosition);
			}
			else
			{
				_animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _ikWeight);
				_animator.SetIKPosition(AvatarIKGoal.LeftFoot, worldConstrainedPosition);
			}
		}


		bool _hasKicked;

		float _ikWeight;
		bool _isInterceptingWithRightFoot;
		float _lastKickTime;
		Vector3 _leftFootPositionLastFrame;
		Vector3 _leftFootVelocity;
		Opponent _opponent;
		Vector3 _rightFootPositionLastFrame;
		Vector3 _rightFootVelocity;
	}
}
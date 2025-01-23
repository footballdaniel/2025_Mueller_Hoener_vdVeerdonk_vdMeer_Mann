using System;
using UnityEngine;

namespace Interactions.Domain.Opponents
{
	[ExecuteInEditMode]
	public class Legs : MonoBehaviour
	{
		[SerializeField] Ball _ball;
		[SerializeField] Animator _animator;
		[SerializeField] Transform _rightFoot;
		[SerializeField] float _startKickDistance = 1.5f;

		public event Action<Vector3> BallIntercepted;

		void Update()
		{
			if (!_ball)
				return;

			_rightFootVelocity = (_rightFootPositionLastFrame - _rightFoot.position) / Time.deltaTime;
			_rightFootPositionLastFrame = _rightFoot.position;

			var distanceBallToFoot = Vector3.Distance(_ball.transform.position, _rightFoot.position);
			var distancePlayerToBall = Vector3.Distance(transform.position, _ball.transform.position);

			// lerp between start kick distance (0) and distance (1)
			var kickProgressPercentage = 1 - Mathf.Clamp01(distancePlayerToBall / _startKickDistance);

			var timeSinceLastKick = Time.time - _lastKickTime;

			if (timeSinceLastKick < 1f)
				kickProgressPercentage = 0;


			if (distanceBallToFoot < 0.2f)
			{
				_hasKicked = true;
				_lastKickTime = Time.time;
				var clampedRightFootVelocity = Vector3.ClampMagnitude(_rightFootVelocity, 5);
				BallIntercepted?.Invoke(clampedRightFootVelocity);
				_opponent.FinishedKicking();
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

			_animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _ikWeight);
			_animator.SetIKPosition(AvatarIKGoal.RightFoot, _ball.transform.position);
		}

		float _ikWeight;
		float _lastKickTime;
		Vector3 _rightFootPositionLastFrame;
		Vector3 _rightFootVelocity;
		bool _hasKicked;
		Opponent _opponent;

		public void Bind(Opponent opponent)
		{
			_opponent = opponent;
		}
	}
}
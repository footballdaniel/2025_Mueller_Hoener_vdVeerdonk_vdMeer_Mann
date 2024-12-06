using System.Collections.Generic;
using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class Opponent : MonoBehaviour
	{
		[SerializeField] Animator _animator;
		[SerializeField] float distanceFromAttacker = 3f;
		[SerializeField] float maxSpeed = 5f;
		[SerializeField] float acceleration = 5f;
		[SerializeField] float maxRotationSpeedDegreesY = 90f;
		[SerializeField] float memoryDuration = 1f;
		[SerializeField] float reactionDelay = 0.3f;

		List<(float time, Vector2 pos)> _attackerPositions = new List<(float time, Vector2 pos)>();

		public void Bind(User user, LeftGoal goalLeft, RightGoal goalRight)
		{
			_user = user;
			_goalLeft = goalLeft;
			_goalRight = goalRight;
		}

		public void Intercept(Ball ball)
		{
			_isIntercepting = true;
			_ball = ball;

			var ballPosition = ball.transform.position;
			var ballDirection = ball.Velocity.normalized;
			var distanceToZAxis = Vector3.ProjectOnPlane(ballPosition - transform.position, Vector3.right);
			var lateralOffset = Mathf.Sign(distanceToZAxis.z) * Mathf.Abs(distanceToZAxis.z);
			_interceptionPoint = ballPosition + ballDirection * lateralOffset;

			var distanceToBall = Vector3.Distance(transform.position, ballPosition);
			if (distanceToBall < 0.4f)
			{
				var kickDirection = transform.forward;
				var kickSpeed = 3f;
				ball.Play(new Pass(kickSpeed, ballPosition, kickDirection));
			}
		}

		void Update()
		{
			var currentTime = Time.time;
			var currentAttackerPos = _user.transform.position;
			_attackerPositions.Add((currentTime, currentAttackerPos));
			while (_attackerPositions.Count > 0 && _attackerPositions[0].time < currentTime - memoryDuration)
				_attackerPositions.RemoveAt(0);

			if (_isIntercepting)
			{
				var ballPosition = _ball.transform.position;
				var ballDirection = _ball.Velocity.normalized;
				_interceptionPoint = ballPosition + ballDirection * (transform.position.x - ballPosition.x);
				MoveTowards(_interceptionPoint);
			}
			else
			{
				var pastAttackerPos = GetAttackerPositionInPast(currentTime - reactionDelay);
				var positionBetweenGoals = (_goalLeft.transform.position + _goalRight.transform.position) / 2;
				var userPosition = new Vector3(pastAttackerPos.x, 0, pastAttackerPos.y);
				var goalToAttackerDirection = (userPosition - positionBetweenGoals).normalized;
				var desiredPosition = userPosition - goalToAttackerDirection * distanceFromAttacker;
				MoveTowards(desiredPosition);

				var lookDirection = userPosition - transform.position;
				lookDirection.y = 0;
				if (lookDirection.sqrMagnitude > 0.001f)
				{
					var targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
					transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationSpeedDegreesY * Time.deltaTime);
				}
			}
		}

		Vector2 GetAttackerPositionInPast(float targetTime)
		{
			if (_attackerPositions.Count == 0)
				return _user.Position;

			if (_attackerPositions[0].time > targetTime)
				return _attackerPositions[0].pos;

			for (var i = 0; i < _attackerPositions.Count - 1; i++)
			{
				var (t1, p1) = _attackerPositions[i];
				var (t2, p2) = _attackerPositions[i + 1];
				if (t1 <= targetTime && t2 >= targetTime)
				{
					var alpha = (targetTime - t1) / (t2 - t1);
					return Vector2.Lerp(p1, p2, alpha);
				}
			}

			return _attackerPositions[_attackerPositions.Count - 1].pos;
		}

		void MoveTowards(Vector3 targetPosition)
		{
			var targetDirection = targetPosition - transform.position;
			targetDirection.y = 0;
			var distanceToTarget = targetDirection.magnitude;

			if (distanceToTarget > 0.25f)
			{
				targetDirection.Normalize();
				var targetVelocity = targetDirection * maxSpeed;
				_currentVelocity = Vector3.MoveTowards(_currentVelocity, targetVelocity, acceleration * Time.deltaTime);
				transform.position += _currentVelocity * Time.deltaTime;
				var localVelocity = transform.InverseTransformDirection(_currentVelocity);
				_animator.SetFloat(VelocityX, localVelocity.x);
				_animator.SetFloat(VelocityY, localVelocity.z);
			}
			else
			{
				_currentVelocity = Vector3.zero;
				_animator.SetFloat(VelocityX, 0);
				_animator.SetFloat(VelocityY, 0);
			}
		}

		readonly static int VelocityX = Animator.StringToHash("VelocityX");
		readonly static int VelocityY = Animator.StringToHash("VelocityY");

		Vector3 _currentVelocity = Vector3.zero;
		Vector3 _interceptionPoint = Vector3.zero;
		LeftGoal _goalLeft;
		RightGoal _goalRight;
		User _user;
		Ball _ball;
		bool _isIntercepting;
	}
}

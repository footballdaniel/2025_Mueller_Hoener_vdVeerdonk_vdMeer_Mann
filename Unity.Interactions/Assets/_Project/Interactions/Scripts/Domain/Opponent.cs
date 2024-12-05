using UnityEngine;

namespace Interactions.Domain
{
	public class Opponent : MonoBehaviour
	{
		[SerializeField] Animator _animator;

		[SerializeField] float distanceFromAttacker = 3f;
		[SerializeField] float maxSpeed = 5f;
		[SerializeField] float acceleration = 5f;
		[SerializeField] float maxRotationSpeedDegreesY = 90f;

		public void Bind(IUser user, LeftGoal goalLeft, RightGoal goalRight)
		{
			_user = user;
			_goalLeft = goalLeft;
			_goalRight = goalRight;
		}

		public void Intercept(Ball ball)
		{
			_isIntercepting = true;
			_ball = ball;

			// Estimate the interception point using the ball's velocity and current position
			var ballPosition = ball.transform.position;
			var ballDirection = ball.Velocity.normalized;

			// Project the ball's path onto the z-axis relative to the opponent's position
			var distanceToZAxis = Vector3.ProjectOnPlane(ballPosition - transform.position, Vector3.right);
			var lateralOffset = Mathf.Sign(distanceToZAxis.z) * Mathf.Abs(distanceToZAxis.z);

			_interceptionPoint = ballPosition + ballDirection * lateralOffset;
		}

		void Update()
		{
			if (_isIntercepting)
			{
				var ballPosition = _ball.transform.position;
				var ballDirection = _ball.Velocity.normalized;

				// Update the interception point dynamically
				_interceptionPoint = ballPosition + ballDirection * (transform.position.x - ballPosition.x);

				MoveTowards(_interceptionPoint);
			}
			else
			{
				var positionBetweenGoals = (_goalLeft.transform.position + _goalRight.transform.position) / 2;
				var userPosition = new Vector3(_user.Position.x, 0, _user.Position.y);

				// Calculate desired position between the goals and user
				var goalToAttackerDirection = (userPosition - positionBetweenGoals).normalized;
				var desiredPosition = userPosition - goalToAttackerDirection * distanceFromAttacker;

				MoveTowards(desiredPosition);

				// Rotate to face the attacker
				var lookDirection = userPosition - transform.position;
				lookDirection.y = 0;
				if (lookDirection.sqrMagnitude > 0.001f)
				{
					var targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
					transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationSpeedDegreesY * Time.deltaTime);
				}
			}
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
		IUser _user;
		Ball _ball;
		bool _isIntercepting;
	}
}

using UnityEngine;

namespace Interactions.Animations
{
	public class BasicBlendspaceAnimations : MonoBehaviour
	{
		[SerializeField] Animator _animator;
		[SerializeField] Transform _attacker;
		[SerializeField] Transform _goal;

		[SerializeField] float distanceFromAttacker = 2f;
		[SerializeField] float maxSpeed = 5f;
		[SerializeField] float acceleration = 5f;
		[SerializeField] float maxRotationSpeedDegreesY = 90f;

		Vector3 _currentVelocity = Vector3.zero;

		void Update()
		{
			// Calculate the direction from the goal to the attacker
			var goalToAttackerDirection = (_attacker.position - _goal.position).normalized;

			// Calculate the desired position 2 meters away from the attacker, on the line between attacker and goal
			var desiredPosition = _attacker.position - goalToAttackerDirection * distanceFromAttacker;

			// Compute movement direction towards the desired position
			var targetDirection = desiredPosition - transform.position;
			targetDirection.y = 0; // Keep movement on the horizontal plane
			var distanceToTarget = targetDirection.magnitude;

			// Update velocity with acceleration
			if (distanceToTarget > 0.1f) // Small threshold to prevent jitter
			{
				targetDirection.Normalize();
				var targetVelocity = targetDirection * maxSpeed;
				_currentVelocity = Vector3.MoveTowards(_currentVelocity, targetVelocity, acceleration * Time.deltaTime);

				transform.position += _currentVelocity * Time.deltaTime;

				// Set animator parameters based on local velocity
				var localVelocity = transform.InverseTransformDirection(_currentVelocity);
				_animator.SetFloat(VelocityX, localVelocity.x);
				_animator.SetFloat(VelocityY, localVelocity.z);
			}
			else
			{
				// Stop the velocity and animator parameters when target position is reached
				_currentVelocity = Vector3.zero;
				_animator.SetFloat(VelocityX, 0);
				_animator.SetFloat(VelocityY, 0);
			}

			// Rotate to face the attacker
			var lookDirection = _attacker.position - transform.position;
			lookDirection.y = 0; // Ensure rotation stays on the horizontal plane

			if (lookDirection.sqrMagnitude > 0.001f) // Prevent jitter when very close
			{
				var targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationSpeedDegreesY * Time.deltaTime);
			}
		}

		readonly static int VelocityX = Animator.StringToHash("VelocityX");
		readonly static int VelocityY = Animator.StringToHash("VelocityY");
	}
}
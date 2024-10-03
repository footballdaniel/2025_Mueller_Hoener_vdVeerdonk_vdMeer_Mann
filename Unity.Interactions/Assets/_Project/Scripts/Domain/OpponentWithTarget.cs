using UnityEngine;

namespace Domain
{
	public class OpponentWithTarget : Opponent
	{
		[SerializeField] Transform _defendTarget; // Assigned via Inspector

		float _currentSpeed = 0f;
		IUser _user;

		void Start()
		{
			_defendTarget = GameObject.Find("Goal").transform;
		}

		void Update()
		{
			if (_user == null || _defendTarget == null) return;

			// Get the positions
			var userPosition = new Vector3(_user.Position.x, 0, _user.Position.y);
			var opponentPosition = transform.position;
			var defendTargetPosition = _defendTarget.position;

			// Calculate the direction from the user to the defend target
			var userToTargetDir = (defendTargetPosition - userPosition).normalized;

			// Calculate the desired position along the line at the specified interpersonal distance
			var desiredPosition = userPosition + userToTargetDir * _interpersonalDistance;

			// Calculate the movement direction and distance
			var moveDirection = desiredPosition - opponentPosition;
			var distance = moveDirection.magnitude;

			if (distance > 0.1f)
			{
				// Handle acceleration and deceleration
				if (distance > _decelerationStartDistance)
					_currentSpeed = Mathf.MoveTowards(_currentSpeed, _maxSpeed, _acceleration * Time.deltaTime);
				else
				{
					var speedFactor = distance / _decelerationStartDistance;
					var targetSpeed = Mathf.Lerp(0, _maxSpeed, speedFactor);
					_currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, _acceleration * Time.deltaTime);
				}

				// Move the opponent towards the desired position
				var velocity = moveDirection.normalized * _currentSpeed;
				transform.position += velocity * Time.deltaTime;

				// Rotate the opponent to face the user
				var lookDirection = userPosition - opponentPosition;

				if (lookDirection.sqrMagnitude > 0.001f)
				{
					var targetRotation = Quaternion.LookRotation(lookDirection);
					transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
				}
			}
			else
				_currentSpeed = 0;
		}

		// Method to set the user
		public void Set(IUser user)
		{
			_user = user;
		}
	}
}
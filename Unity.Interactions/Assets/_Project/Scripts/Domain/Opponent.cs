using UnityEngine;

public class Opponent : MonoBehaviour
{
	[SerializeField] float _maxSpeed = 5f;
	[SerializeField] float _acceleration = 3f;
	[SerializeField] float _interpersonalDistance = 3f; // Target distance to the user
	[SerializeField] float _decelerationStartDistance = 5f; // Distance at which deceleration starts

	float _currentSpeed = 0f;
	IUser _user;

	void Update()
	{
		if (_user == null) return;

		// Get direction towards the user
		var direction = _user.Position - new Vector2(transform.position.x, transform.position.z);
		var distance = direction.magnitude;

		if (distance > _interpersonalDistance)
		{
			if (distance > _decelerationStartDistance)
			{
				// Accelerate towards the user if farther than the deceleration start distance
				_currentSpeed = Mathf.MoveTowards(_currentSpeed, _maxSpeed, _acceleration * Time.deltaTime);
			}
			else
			{
				// Decelerate as we approach the interpersonal distance
				var speedFactor = (distance - _interpersonalDistance) / (_decelerationStartDistance - _interpersonalDistance);
				var targetSpeed = Mathf.Lerp(0, _maxSpeed, speedFactor); // Interpolate speed based on proximity
				_currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, _acceleration * Time.deltaTime);
			}

			// Move towards the user using the calculated speed
			var velocity = direction.normalized * _currentSpeed;
			transform.position += new Vector3(velocity.x, 0, velocity.y) * Time.deltaTime;

			// Rotate towards the user
			var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0, angle, 0);
		}
		else
		{
			// Stop moving when the interpersonal distance is reached
			_currentSpeed = 0;
		}
	}

	public void Set(IUser user)
	{
		_user = user;
	}
}
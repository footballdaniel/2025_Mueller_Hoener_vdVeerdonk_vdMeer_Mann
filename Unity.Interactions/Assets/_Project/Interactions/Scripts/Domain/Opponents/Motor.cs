using Interactions.Domain.Opponents;
using UnityEngine;

public class Motor
{
	public Motor(float maxSpeed, float acceleration, float maxRotationSpeedDegreesY)
	{
		_maxSpeed = maxSpeed;
		_acceleration = acceleration;
		_maxRotationSpeedDegreesY = maxRotationSpeedDegreesY;
	}

	public Vector2 Velocity { get; private set; }

	public Vector3 Move(InformationSources sources, Vector3 currentPosition, Quaternion currentRotation, float deltaTime)
	{
		var finalPos = sources.Combine();
		var targetDirection = finalPos - currentPosition;
		targetDirection.y = 0;
		var distanceToTarget = targetDirection.magnitude;

		var targetSpeed = _maxSpeed;

		if (distanceToTarget < 0.25f)
			targetSpeed *= distanceToTarget / 0.25f;

		if (distanceToTarget > 0.0001f)
			targetDirection.Normalize();
		else
			targetDirection = Vector3.zero;

		var desiredVelocity3D = Vector3.MoveTowards(_currentVelocity, targetDirection * targetSpeed, _acceleration * deltaTime);

		var localVelocity2D = new Vector2(
			Vector3.Dot(desiredVelocity3D, currentRotation * Vector3.right),
			Vector3.Dot(desiredVelocity3D, currentRotation * Vector3.forward)
		);

		Velocity = localVelocity2D;
		_currentVelocity = desiredVelocity3D;

		var nextPosition = currentPosition + desiredVelocity3D * deltaTime;
		return nextPosition;
	}

	public Quaternion Rotate(InformationSources sources, Vector3 currentPosition, Quaternion currentRotation, float deltaTime)
	{
		var finalPos = sources.Combine();
		var lookDirection = finalPos - currentPosition;
		lookDirection.y = 0;

		if (lookDirection.sqrMagnitude <= 0.001f)
			return currentRotation;

		var localForward = currentRotation * Vector3.forward;
		var angleToTarget = Vector3.SignedAngle(localForward, lookDirection.normalized, Vector3.up);
		var maxRotationDelta = _maxRotationSpeedDegreesY * deltaTime;
		var clampedAngle = Mathf.Clamp(angleToTarget, -maxRotationDelta, maxRotationDelta);

		return Quaternion.Euler(0, currentRotation.eulerAngles.y + clampedAngle, 0);
	}

	readonly float _acceleration;
	Vector3 _currentVelocity = Vector3.zero;
	readonly float _maxRotationSpeedDegreesY;
	readonly float _maxSpeed;
}
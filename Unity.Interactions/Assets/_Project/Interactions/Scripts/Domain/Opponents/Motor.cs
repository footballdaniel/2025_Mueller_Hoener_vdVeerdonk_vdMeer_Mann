using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class Motor
	{
		readonly float _acceleration;
		Vector3 _currentVelocity = Vector3.zero;
		readonly float _maxRotationSpeedDegreesY;
		readonly float _maxSpeed;

		public Motor(float maxSpeed, float acceleration, float maxRotationSpeedDegreesY)
		{
			_maxSpeed = maxSpeed;
			_acceleration = acceleration;
			_maxRotationSpeedDegreesY = maxRotationSpeedDegreesY;
		}

		public Vector2 LocalVelocity { get; private set; }
		public Vector2 Velocity { get; private set; }

		public Vector3 Move(InformationSources sources, Vector3 currentPosition, Quaternion currentRotation, float deltaTime)
		{
			var finalPos = sources.CombinePositions();
			var targetDirection = finalPos - currentPosition;
			targetDirection.y = 0;
			var distanceToTarget = targetDirection.magnitude;

			var ratio = Mathf.Clamp01(distanceToTarget / 1f);
			var easedRatio = ratio * ratio * ratio;
			var targetSpeed = _maxSpeed * easedRatio;

			if (distanceToTarget > 0.0001f)
				targetDirection.Normalize();
			else
				targetDirection = Vector3.zero;

			var desiredVelocity = targetDirection * targetSpeed;
			var desiredVelocity3D = Vector3.MoveTowards(_currentVelocity, desiredVelocity, _acceleration * deltaTime);
			var step = desiredVelocity3D * deltaTime;

			if (step.magnitude > distanceToTarget)
				desiredVelocity3D = targetDirection * (distanceToTarget / deltaTime);

			Velocity = new Vector2(desiredVelocity3D.x, desiredVelocity3D.z);			
			var localVelocity2D = new Vector2(
				Vector3.Dot(desiredVelocity3D, currentRotation * Vector3.right),
				Vector3.Dot(desiredVelocity3D, currentRotation * Vector3.forward)
			);
			LocalVelocity = localVelocity2D;
			_currentVelocity = desiredVelocity3D;
			return currentPosition + desiredVelocity3D * deltaTime;
		}

		public Quaternion Rotate(InformationSources sources, Vector3 currentPosition, Quaternion currentRotation, float deltaTime)
		{
			var finalY = sources.CombineRotationsY();
			if (Mathf.Abs(finalY) < 0.0001f)
			{
				var finalPos = sources.CombinePositions();
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
			else
			{
				var currentY = currentRotation.eulerAngles.y;
				var angle = Mathf.DeltaAngle(currentY, finalY);
				var rotationDelta = Mathf.Clamp(angle, -_maxRotationSpeedDegreesY * deltaTime, _maxRotationSpeedDegreesY * deltaTime);
				return Quaternion.Euler(0, currentY + rotationDelta, 0);
			}
		}

	}
}

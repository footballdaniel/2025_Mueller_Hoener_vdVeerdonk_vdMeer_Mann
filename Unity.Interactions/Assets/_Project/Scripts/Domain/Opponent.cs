using UnityEngine;

namespace Domain
{
	public class Opponent : MonoBehaviour
	{
		[SerializeField] float _maxSpeed = 5f;
		[SerializeField] float _acceleration = 3f;
		[SerializeField] float _interpersonalDistance = 3f;
		[SerializeField] float _decelerationStartDistance = 5f;

		float _currentSpeed = 0f;
		IUser _user;
		Vector3 _targetPosition;

		void Update()
		{
			if (_user == null) return;

			var userPosition = new Vector3(_user.Position.x, 0, _user.Position.y);
			var direction = userPosition - transform.position;
			var distance = direction.magnitude;

			_targetPosition = userPosition - direction.normalized * _interpersonalDistance;

			if (distance > _interpersonalDistance)
			{
				if (distance > _decelerationStartDistance)
				{
					_currentSpeed = Mathf.MoveTowards(_currentSpeed, _maxSpeed, _acceleration * Time.deltaTime);
				}
				else
				{
					var speedFactor = (distance - _interpersonalDistance) / (_decelerationStartDistance - _interpersonalDistance);
					var targetSpeed = Mathf.Lerp(0, _maxSpeed, speedFactor);
					_currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, _acceleration * Time.deltaTime);
				}

				var velocity = (new Vector3(_targetPosition.x, 0, _targetPosition.z) - transform.position).normalized * _currentSpeed;
				transform.position += velocity * Time.deltaTime;

				var angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Euler(0, angle, 0);
			}
			else
			{
				_currentSpeed = 0;
			}
		}

		public void Set(IUser user)
		{
			_user = user;
		}
	}
}
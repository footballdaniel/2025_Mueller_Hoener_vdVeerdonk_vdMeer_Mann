using UnityEngine;
using Random = System.Random;

namespace Domain
{
	public class Opponent : MonoBehaviour
	{
		[SerializeField] protected float _maxSpeed = 5f;
		[SerializeField] protected float _acceleration = 3f;
		[SerializeField] protected float _interpersonalDistance = 3.5f;
		[SerializeField] protected float _decelerationStartDistance = 5f;

		float _currentSpeed = 0f;
		bool _isGoingRight;
		Vector3 _targetPosition;
		IUser _user;

		void Start()
		{
			// random boolean
			var random = new Random();
			_isGoingRight = random.Next(0, 2) == 0;
		}

		void Update()
		{
			if (_user == null) return;

			var userPosition = new Vector3(_user.Position.x, 0, _user.Position.y);
			var direction = userPosition - transform.position;
			var distance = direction.magnitude;

			_targetPosition = userPosition - direction.normalized * _interpersonalDistance;

			// add 0.5m to target position based on bool
			if (_isGoingRight)
				_targetPosition.z += 0.4f;
			else
				_targetPosition.z -= 0.4f;

			if (distance > _interpersonalDistance)
			{
				if (distance > _decelerationStartDistance)
					_currentSpeed = Mathf.MoveTowards(_currentSpeed, _maxSpeed, _acceleration * Time.deltaTime);
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
				_currentSpeed = 0;
		}

		public void Set(IUser user)
		{
			_user = user;
		}
	}
}
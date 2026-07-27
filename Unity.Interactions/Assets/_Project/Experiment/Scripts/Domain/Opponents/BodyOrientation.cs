using UnityEngine;

namespace Interactions.Domain.Opponents
{
	[ExecuteInEditMode]
	internal class BodyOrientation : MonoBehaviour
	{
		[Header("Dependencies, can be bound dynamically")]
		[SerializeField] Transform _headBone;
		[SerializeField] Transform _target;

		[Header("Head")]
		[SerializeField] float _maxHeadAngleDegrees = 90f;
		[SerializeField] float _headSpeedDegreesPerSecond = 100f;

		void LateUpdate()
		{
			if (_target == null || _headBone == null)
				return;

			var toTarget = _target.position - _headBone.position;
			if (toTarget.sqrMagnitude < 0.0001f)
				return;

			var lookDirection = Vector3.RotateTowards(transform.forward, toTarget.normalized, _maxHeadAngleDegrees * Mathf.Deg2Rad, 0f);

			var lookingStraightForwardOrientation = Quaternion.Euler(0, 90, -90);
			var targetRotation = Quaternion.LookRotation(lookDirection) * lookingStraightForwardOrientation;

			if (!_isTurning)
			{
				_headRotation = _headBone.rotation;
				_isTurning = true;
			}

			_headRotation = Quaternion.RotateTowards(_headRotation, targetRotation, _headSpeedDegreesPerSecond * Time.deltaTime);
			_headBone.rotation = _headRotation;
		}

		public void LookAt(Transform target)
		{
			_target = target;
			_isTurning = false;
		}

		public void LookStraightAhead()
		{
			_target = null;
			_isTurning = false;
		}

		Quaternion _headRotation;
		bool _isTurning;
	}
}

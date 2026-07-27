using UnityEngine;

namespace Interactions.Domain.Opponents
{
	/// Drives the head bone to look at a target, on top of whatever the body is doing.
	/// The neck limit is measured against the body's current facing, not a fixed world
	/// direction, so in the proactive condition - where the body is deliberately rotated
	/// away from the user - the head still turns back to face the user.
	[ExecuteInEditMode]
	internal class BodyOrientation : MonoBehaviour
	{
		[Header("Dependencies, can be bound dynamically")]
		[SerializeField] Transform _headBone;
		[SerializeField] Transform _target;

		[Header("Neck")]
		[Tooltip("How far the head may turn away from the body's facing. The proactive condition offsets the body by up to lateralOffsetMax * bodyOrientationDegreesPerMeter degrees, so this has to be at least that large for the head to fully reach the user.")]
		[SerializeField] float _maxNeckAngleDegrees = 90f;
		[SerializeField] float _neckSpeedDegreesPerSecond = 100f;

		void LateUpdate()
		{
			if (_target == null || _headBone == null)
				return;

			var toTarget = _target.position - _headBone.position;
			if (toTarget.sqrMagnitude < 0.0001f)
				return;

			// Clamping against transform.forward - the body's live orientation - is what
			// keeps the head on the user when the body yaws away.
			var lookDirection = Vector3.RotateTowards(transform.forward, toTarget.normalized, _maxNeckAngleDegrees * Mathf.Deg2Rad, 0f);

			var lookingStraightForwardOrientation = Quaternion.Euler(0, 90, -90);
			var targetRotation = Quaternion.LookRotation(lookDirection) * lookingStraightForwardOrientation; // compensate for the head bone's orientation

			// Turn our own state towards the target rather than the bone's current rotation.
			// The Animator rewrites the head bone every frame before LateUpdate, so slewing
			// from _headBone.rotation would restart from the animated pose each frame and
			// never travel more than one frame's worth of degrees away from it.
			if (!_isTurning)
			{
				_neckRotation = _headBone.rotation;
				_isTurning = true;
			}

			_neckRotation = Quaternion.RotateTowards(_neckRotation, targetRotation, _neckSpeedDegreesPerSecond * Time.deltaTime);
			_headBone.rotation = _neckRotation;
		}

		public void LookAt(Transform target)
		{
			_target = target;
			_isTurning = false; // pick the turn up from wherever the animation currently has the head
		}

		public void LookStraightAhead()
		{
			_target = null;
			_isTurning = false;
		}

		Quaternion _neckRotation;
		bool _isTurning;
	}
}

using UnityEngine;

namespace Interactions.Domain.Opponents
{
	[ExecuteInEditMode]
	internal class BodyOrientation : MonoBehaviour
	{
		[Header("Dependencies, can be bound dynamically")]
		[SerializeField] Transform _headBone;
		[SerializeField] Transform _target;

		void LateUpdate()
		{
			if (_target != null)
			{
				var lookingStraightForwardOrientation = Quaternion.Euler(0, 90, -90);
				var targetRotation = Quaternion.LookRotation(_target.position - _headBone.position);
				targetRotation *= lookingStraightForwardOrientation; // compensate for the head bone's orientation
				targetRotation = Quaternion.RotateTowards(_headBone.rotation, targetRotation, 100f * Time.deltaTime);
				targetRotation = Quaternion.RotateTowards(Quaternion.Euler(0,0,-90), targetRotation, 30f); // clamp to 30 degrees
				_headBone.rotation = targetRotation;
			}
		}

		public void LookAt(Transform target)
		{
			_target = target;
		}
		
		public void LookStraightAhead()
		{
			_target = null;
		}
	}
}
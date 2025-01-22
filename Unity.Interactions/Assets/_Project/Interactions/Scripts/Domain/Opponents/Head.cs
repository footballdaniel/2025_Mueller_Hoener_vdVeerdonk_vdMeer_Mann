using UnityEngine;

namespace Interactions.Domain.Opponents
{
	internal class Head : MonoBehaviour
	{
		[SerializeField] Transform _headBone;

		void LateUpdate()
		{
			if (_target != null)
			{
				// wrong way round 180 degrees. 
				// plus also on the ground, and i need at 1.8m height... so maybe pass the users head through instead of the user?
				var targetRotation = Quaternion.LookRotation(_target.position - _headBone.position);
				var limitedRotation = Quaternion.RotateTowards(_headBone.rotation, targetRotation, 100f * Time.deltaTime);
				// var clampedRotation = Quaternion.RotateTowards(Quaternion.identity, limitedRotation, 30f);
				_headBone.rotation = targetRotation;
			}
		}

		public void LookAt(Transform target)
		{
			_target = target;
		}

		Transform _target;
	}
}
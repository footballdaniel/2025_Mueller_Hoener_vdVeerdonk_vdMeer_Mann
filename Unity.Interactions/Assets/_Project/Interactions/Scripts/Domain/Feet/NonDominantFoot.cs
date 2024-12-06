using UnityEngine;

namespace Interactions.Domain
{
	public class NonDominantFoot : MonoBehaviour
	{
		[SerializeField] XRTracker _nonDominantFootTracker;

		public Vector3 Velocity => _velocity;

		void Start()
		{
			transform.parent = _nonDominantFootTracker.gameObject.transform;
			_previousPosition = transform.position;
		}

		void Update()
		{
			var currentPosition = transform.position;
			_velocity = (currentPosition - _previousPosition) / Time.deltaTime;
			_previousPosition = currentPosition;
		}

		Vector3 _previousPosition;
		Vector3 _velocity;
	}
}
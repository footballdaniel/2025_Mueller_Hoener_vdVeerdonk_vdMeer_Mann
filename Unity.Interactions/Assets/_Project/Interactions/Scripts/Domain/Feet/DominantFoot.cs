using UnityEngine;

namespace Interactions.Domain.Feet
{
	public class DominantFoot : MonoBehaviour
	{
		[Header("Dependencies"), SerializeField] XRTracker _dominantFootTracker;
		[SerializeField] Transform _targetLocation;

		[field: SerializeReference] public float Speed { get; private set; }
		[field: SerializeReference] public float VelocityTowardsTarget { get; private set; }
		[field: SerializeReference] public Vector3 Velocity { get; private set; }

		void Start()
		{
			transform.parent = _dominantFootTracker.gameObject.transform;
			_positionLastFrame = transform.position;
		}

		void Update()
		{
			Velocity = (transform.position - _positionLastFrame) / Time.deltaTime;
			Speed = Velocity.magnitude;
			VelocityTowardsTarget = Vector3.Dot(Velocity, _targetLocation.forward);

			_positionLastFrame = transform.position;
		}

		bool _isKicking;

		Vector3 _positionLastFrame;
	}
}
using System;
using _Project.Interactions.Scripts.Domain;
using UnityEngine;

namespace Interactions.Scripts.Domain
{
	public class DominantFoot : MonoBehaviour
	{
		[Header("Dependencies"), SerializeField] XRTracker _dominantFootTracker;
		[SerializeField] Transform _targetLocation;

		[field: SerializeReference] public float Speed { get; private set; }
		[field: SerializeReference] public float VelocityTowardsTarget { get; private set; }
		bool _isKicking;

		[Header("Settings")] float _kickThresholdSpeed = 2f;


		Vector3 _positionLastFrame;
		float _velocityLastFrame;

		void Start()
		{
			transform.parent = _dominantFootTracker.gameObject.transform;
			_positionLastFrame = transform.position;
		}

		void Update()
		{
			var velocity = (transform.position - _positionLastFrame) / Time.deltaTime;
			Speed = velocity.magnitude;
			VelocityTowardsTarget = Vector3.Dot(velocity, _targetLocation.forward);

			_positionLastFrame = transform.position;

			DetectPassWhenVelocityAboveTargetAndAfterPeakVelocity(velocity);
		}

		public event Action<Pass> Passed;

		void DetectPassWhenVelocityAboveTargetAndAfterPeakVelocity(Vector3 velocity)
		{
			if (VelocityTowardsTarget > _kickThresholdSpeed)
			{
				_isKicking = true;

				if (_velocityLastFrame > VelocityTowardsTarget)
				{
					Passed?.Invoke(new Pass(Speed, transform.position, velocity.normalized));
					Debug.Log("Kick detected with speed: " + Speed + " and velocity: " + velocity);
				}

				_velocityLastFrame = VelocityTowardsTarget;
			}
		}
	}
}
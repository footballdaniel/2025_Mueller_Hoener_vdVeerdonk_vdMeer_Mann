using System;
using UnityEngine;

namespace Domain
{
	public class DominantFoot : MonoBehaviour
	{
		public event Action<Pass> Passed;
	
		[Header("Settings")] float _kickThresholdSpeed = 2f;
	
		[Header("Dependencies"), SerializeField] XRTracker _dominantFoot;
		[SerializeField] Transform _targetLocation;
	
		[field: SerializeReference] public float Speed { get; private set; }
		[field: SerializeReference] public float VelocityTowardsTarget { get; private set; }

		void Start()
		{
			transform.parent = _dominantFoot.gameObject.transform;
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


		Vector3 _positionLastFrame;
		bool _isKicking;
		float _velocityLastFrame;
	}
}
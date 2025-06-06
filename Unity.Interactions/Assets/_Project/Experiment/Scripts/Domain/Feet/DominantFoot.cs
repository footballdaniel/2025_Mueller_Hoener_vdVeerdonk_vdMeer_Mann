using System.Collections.Generic;
using UnityEngine;

namespace Interactions.Domain.Feet
{
	public class DominantFoot : MonoBehaviour
	{
		const int FrameWindow = 5;
		[Header("Dependencies"), SerializeField] XRTracker _dominantFootTracker;
		[field: SerializeReference] public float Speed { get; private set; }
		[field: SerializeReference] public Vector3 Velocity { get; private set; }

		void Start()
		{
			transform.parent = _dominantFootTracker.gameObject.transform;
			_positionLastFrame = transform.position;
		}

		void Update()
		{
			// Calculate current frame velocity
			var currentVelocity = (transform.position - _positionLastFrame) / Time.deltaTime;

			// Update the velocity history
			_velocityHistory.Enqueue(currentVelocity);

			if (_velocityHistory.Count > FrameWindow)
				_velocityHistory.Dequeue();

			// Apply moving average filter
			var averagedVelocity = Vector3.zero;

			foreach (var velocity in _velocityHistory)
				averagedVelocity += velocity;
			averagedVelocity /= _velocityHistory.Count;

			// Set the filtered velocity and speed
			Velocity = averagedVelocity;
			Speed = Velocity.magnitude;

			// Update position for the next frame
			_positionLastFrame = transform.position;
		}

		Vector3 _positionLastFrame;

		Queue<Vector3> _velocityHistory = new();
	}
}
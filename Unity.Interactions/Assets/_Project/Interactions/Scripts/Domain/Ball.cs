using UnityEngine;

namespace Interactions.Domain
{
	public class Ball : MonoBehaviour
	{
		[SerializeField] float _energyTransferCoefficient = 2f;
		[SerializeField] Rigidbody _rigidbody;

		public Vector3 Velocity => _rigidbody.linearVelocity;
		
		public void Play(Pass pass)
		{
			var optimalPassVelocity = 15f;
			var speed = Mathf.Lerp(pass.Speed, optimalPassVelocity, 0.75f);
			var passDirection = new Vector3(pass.Direction.x, pass.Direction.z/5f, pass.Direction.z);
			
			_rigidbody.linearVelocity = passDirection * speed * _energyTransferCoefficient;
		}
	}
}
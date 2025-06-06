using UnityEngine;

namespace Interactions.Domain
{
	public class Ball : MonoBehaviour
	{
		[SerializeField] float _energyTransferCoefficient = 5f;
		[SerializeField] Rigidbody _rigidbody;

		public Vector3 Velocity => _rigidbody.linearVelocity;

		public void Play(Pass pass)
		{
			var passDirection = new Vector3(pass.Direction.x, 0, pass.Direction.z);

			_rigidbody.linearVelocity = passDirection * pass.KickVelocity * _energyTransferCoefficient;
		}
	}
}
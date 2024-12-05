using UnityEngine;

namespace Interactions.Domain
{
	public class Ball : MonoBehaviour
	{
		[SerializeField] float _energyTransferCoefficient = 1.5f;
		[SerializeField] Rigidbody _rigidbody;

		public void Play(Pass pass)
		{
			_rigidbody.linearVelocity = pass.Direction * pass.Speed * _energyTransferCoefficient;
		}
		
	}
}
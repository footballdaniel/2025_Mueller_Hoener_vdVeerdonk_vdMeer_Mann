using _Project.Scripts.Domain;
using UnityEngine;
using UnityEngine.Serialization;

namespace System.Runtime.CompilerServices
{
	public class Ball : MonoBehaviour
	{
		[SerializeField] float _energyTransferCoefficient = 1.5f;
		[SerializeField] Rigidbody _rigidbody;

		public void Set(Pass pass)
		{
			_rigidbody.linearVelocity = pass.Direction * pass.Speed * _energyTransferCoefficient;
		}
		
	}
}
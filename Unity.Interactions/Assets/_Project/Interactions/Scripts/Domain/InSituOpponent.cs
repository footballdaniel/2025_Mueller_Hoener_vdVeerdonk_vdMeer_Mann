using UnityEngine;

namespace Interactions.Domain
{
	public class InSituOpponent : MonoBehaviour
	{
		[Header("Dependencies"), SerializeField] XRTracker _hipsTracker;

		public Vector3 Hips => _hipsTracker.transform.position;

		void Start()
		{
			transform.parent = _hipsTracker.gameObject.transform;
		}
	}
}
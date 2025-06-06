using UnityEngine;

namespace Interactions.Domain
{
	public class TrackedHead : MonoBehaviour
	{
		[SerializeField] XRTracker _headTracker;

		void Start()
		{
			transform.parent = _headTracker.gameObject.transform;
		}
	}
}
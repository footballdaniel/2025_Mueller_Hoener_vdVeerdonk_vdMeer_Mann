using UnityEngine;

namespace Interactions.Domain
{
	public class Head : MonoBehaviour
	{
		[SerializeField] XRTracker _headTracker;

		void Start()
		{
			transform.parent = _headTracker.gameObject.transform;
		}
	}
}
using UnityEngine;

public class Hips : MonoBehaviour
{
	[SerializeField] XRTracker _hipsTracker;
	
	public Vector3 Position => transform.position;

	void Start()
	{
		transform.parent = _hipsTracker.gameObject.transform;
	}
}
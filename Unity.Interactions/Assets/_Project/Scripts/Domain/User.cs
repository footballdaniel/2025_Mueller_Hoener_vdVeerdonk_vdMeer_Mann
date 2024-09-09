using UnityEngine;

public class User : MonoBehaviour, IUser
{
	[Header("External dependencies"), SerializeField] XRTracker _tracker;
	
	
	void Start()
	{
		transform.parent = _tracker.gameObject.transform;
	}

	public Vector2 Position => new(transform.position.x, transform.position.z);
}
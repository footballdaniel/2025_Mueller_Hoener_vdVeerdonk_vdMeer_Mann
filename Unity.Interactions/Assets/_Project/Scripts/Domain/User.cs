using UnityEngine;

namespace Domain
{
	public class User : MonoBehaviour, IUser
	{
		[Header("External dependencies"), SerializeField] XRTracker _headTracker;
		[SerializeField] DominantFoot _dominantFoot;
	
		void Start()
		{
			transform.parent = _headTracker.gameObject.transform;
		}
	

		public Vector2 Position => new(transform.position.x, transform.position.z);
	}
}
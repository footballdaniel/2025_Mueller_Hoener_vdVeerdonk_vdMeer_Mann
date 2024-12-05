using Interactions.Scripts.Domain;
using UnityEngine;

namespace _Project.Interactions.Scripts.Domain
{
	public class User : MonoBehaviour, IUser
	{
		[Header("External dependencies")] 
		[field: SerializeReference] public DominantFoot DominantFoot { get; private set; }
		[field: SerializeReference] public NonDominantFoot NonDominantFoot { get; private set; }
		[field: SerializeReference] public Head Head { get; private set; }
	
		void Start()
		{
			transform.parent = Head.transform;
		}
	

		public Vector2 Position => new(transform.position.x, transform.position.z);
	}
}
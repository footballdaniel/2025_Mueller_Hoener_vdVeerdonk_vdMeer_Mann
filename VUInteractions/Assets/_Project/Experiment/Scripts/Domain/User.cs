using Interactions.Domain.Feet;
using UnityEngine;

namespace Interactions.Domain
{
	public class User : MonoBehaviour
	{
		[Header("External dependencies")]
		[field: SerializeReference] public DominantFoot DominantFoot { get; private set; }

		[field: SerializeReference] public NonDominantFoot NonDominantFoot { get; private set; }
		[field: SerializeReference] public TrackedHead TrackedHead { get; private set; }
		[field: SerializeReference] public Hips Hips { get; private set; }

		void Start()
		{
			transform.parent = TrackedHead.transform;
		}

		public Vector2 Position => new(transform.position.x, transform.position.z);
	}
}
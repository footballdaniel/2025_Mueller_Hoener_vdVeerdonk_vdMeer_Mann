using UnityEngine;

namespace Interactions.Domain.Goals
{
	public class RightGoal : MonoBehaviour
	{
		public void PlaceWithDistance(float newDistance)
		{
			transform.position = new Vector3(transform.position.x, 0, -newDistance);
		}
	}
}
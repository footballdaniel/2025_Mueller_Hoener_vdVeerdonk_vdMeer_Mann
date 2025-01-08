using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class NoFootPerception : IPercept
	{
		public void Tick(float time)
		{
			
		}

		public Vector2 Perceive()
		{
			return Vector2.zero;
		}

		public void ChangeReactionTime(float newReactionTime)
		{
			
		}
	}
}
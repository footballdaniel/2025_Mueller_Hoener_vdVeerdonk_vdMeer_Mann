using UnityEngine;

namespace Interactions.Domain.DecisionMaking.Perceptions
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
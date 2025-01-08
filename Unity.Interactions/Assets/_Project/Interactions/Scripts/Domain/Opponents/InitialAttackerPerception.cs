using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class InitialAttackerPerception : IAttackerPerception
	{
		readonly Vector2 _perceivedPosition;

		public InitialAttackerPerception(User user)
		{
			_perceivedPosition = user.Position;
		}

		public void Tick(float time)
		{
			
		}

		public Vector2 Perceive()
		{
			return _perceivedPosition;
		}

		public void ChangeReactionTime(float newReactionTime)
		{
			
		}
	}
}
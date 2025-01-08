using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public interface IAttackerPerception
	{
		void Tick(float time);
		Vector2 Perceive();
		void ChangeReactionTime(float newReactionTime);
	}
}
using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public interface IPercept
	{
		void Tick(float time);
		Vector2 Perceive();
		void ChangeReactionTime(float newReactionTime);
	}
}
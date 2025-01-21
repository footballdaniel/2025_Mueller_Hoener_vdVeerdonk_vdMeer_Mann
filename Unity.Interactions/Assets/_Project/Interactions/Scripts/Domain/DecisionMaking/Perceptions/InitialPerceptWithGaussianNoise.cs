using UnityEngine;

namespace Interactions.Domain.DecisionMaking.Perceptions
{
	public class InitialPerceptWithGaussianNoise : IPercept
	{
		Vector2 _perceivedPosition;

		public InitialPerceptWithGaussianNoise(User user)
		{
			_perceivedPosition = user.Position;
			OpponentMovesLaterallyWithARandomOffset();
		}

		void OpponentMovesLaterallyWithARandomOffset()
		{
			// add some gaussian noise around the z axis
			var noise = Random.Range(-1f, 1f);
			_perceivedPosition.y += noise;
			Debug.Log($"Initial percept with noise: {_perceivedPosition}");
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
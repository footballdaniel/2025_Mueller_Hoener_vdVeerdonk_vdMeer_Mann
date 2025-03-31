using System.Collections.Generic;
using Interactions.Domain.Feet;
using UnityEngine;

namespace Interactions.Domain.DecisionMaking.Perceptions
{
	public class DelayedFootPerception : IPercept
	{

		public DelayedFootPerception(float duration, float delay, float avgWindow, DominantFoot userDominantFoot, NonDominantFoot userNonDominantFoot)
		{
			_duration = duration;
			_delay = delay;
			_avgWindow = avgWindow;
			_userDominantFoot = userDominantFoot;
			_userNonDominantFoot = userNonDominantFoot;
			_entries = new List<(float time, float velocity)>();
		}

		public void Tick(float time)
		{
			var combinedVelocity = _userDominantFoot.Velocity.z + _userNonDominantFoot.Velocity.z;
			_entries.Add((time, combinedVelocity));

			while (_entries.Count > 0 && _entries[0].time < time - _duration)
				_entries.RemoveAt(0);
		}

		public Vector2 Perceive()
		{
			var targetTime = Time.time - _delay;
			var avg = ApplyMovingAverageFilterAndReturnValue(targetTime, _avgWindow);
			return new Vector2(0, avg);
		}

		public void ChangeReactionTime(float newReactionTime)
		{
			_delay = newReactionTime;
		}

		float ApplyMovingAverageFilterAndReturnValue(float currentTime, float window)
		{
			var startTime = currentTime - window;
			var sum = 0f;
			var count = 0;

			for (var i = 0; i < _entries.Count; i++)
			{
				var (t, v) = _entries[i];

				if (t >= startTime && t <= currentTime)
				{
					sum += v;
					count++;
				}
			}

			return count == 0 ? 0f : sum / count;
		}

		readonly List<(float time, float velocity)> _entries;
		readonly DominantFoot _userDominantFoot;
		readonly NonDominantFoot _userNonDominantFoot;
		float _avgWindow;
		float _delay;
		float _duration;
	}
}
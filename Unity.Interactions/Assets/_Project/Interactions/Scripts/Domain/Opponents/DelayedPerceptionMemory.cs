using System.Collections.Generic;
using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class DelayedPerceptionMemory
	{

		public DelayedPerceptionMemory(float duration, float delay)
		{
			_entries = new List<(float time, Vector2 pos)>();
			_duration = duration;
			_delay = delay;
		}

		public void Add(float time, Vector2 pos)
		{
			_entries.Add((time, pos));

			while (_entries.Count > 0 && _entries[0].time < time - _duration)
				_entries.RemoveAt(0);
		}

		public void ChangeReactionTime(float newReactionTime)
		{
			_delay = newReactionTime;
		}

		public Vector2 Get(float time)
		{
			var targetTime = time - _delay;

			if (_entries.Count == 0)
				return Vector2.zero;

			if (targetTime <= _entries[0].time)
				return _entries[0].pos;

			if (targetTime >= _entries[^1].time)
				return _entries[^1].pos;

			for (var i = 0; i < _entries.Count - 1; i++)
			{
				var (t1, p1) = _entries[i];
				var (t2, p2) = _entries[i + 1];

				if (t1 <= targetTime && t2 >= targetTime)
					return Vector2.Lerp(p1, p2, (targetTime - t1) / (t2 - t1));
			}

			return _entries[^1].pos;
		}

		readonly float _duration;
		readonly List<(float time, Vector2 pos)> _entries;

		float _delay;
	}
}
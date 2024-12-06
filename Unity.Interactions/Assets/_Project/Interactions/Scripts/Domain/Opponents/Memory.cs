using System.Collections.Generic;
using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class Memory
	{

		public Memory(float duration)
		{
			_entries = new List<(float time, Vector2 pos)>();
			_duration = duration;
		}

		public void Add(float time, Vector2 pos)
		{
			_entries.Add((time, pos));

			while (_entries.Count > 0 && _entries[0].time < time - _duration)
				_entries.RemoveAt(0);
		}

		public Vector2 Get(float time)
		{
			if (_entries.Count == 0)
				return Vector2.zero;

			if (_entries[0].time > time)
				return _entries[0].pos;

			for (var i = 0; i < _entries.Count - 1; i++)
			{
				var (t1, p1) = _entries[i];
				var (t2, p2) = _entries[i + 1];

				if (t1 <= time && t2 >= time)
					return Vector2.Lerp(p1, p2, (time - t1) / (t2 - t1));
			}

			return _entries[^1].pos;
		}

		readonly float _duration;
		readonly List<(float time, Vector2 pos)> _entries;
	}
}
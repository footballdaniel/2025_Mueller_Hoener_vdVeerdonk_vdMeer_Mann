using System.Collections.Generic;
using System.Linq;
using _Project.PassDetection.Common;
using UnityEngine;

namespace Interactions.Infra
{
	public class InputDataQueue
	{
		public InputDataQueue(int maxSize = 10)
		{
			_maxSize = maxSize;
		}

		public void EnQueue(Vector3 dominantFootPosition, Vector3 nonDominantFootPosition, float timestamp)
		{
			if (_dominantFootPositions.Count >= _maxSize)
			{
				_dominantFootPositions.Dequeue();
				_nonDominantFootPositions.Dequeue();
				_timestamps.Dequeue();
			}

			_dominantFootPositions.Enqueue(dominantFootPosition);
			_nonDominantFootPositions.Enqueue(nonDominantFootPosition);
			_timestamps.Enqueue(timestamp);
		}

		public Vector3 CalculateGetHighestObservedVelocity()
		{
			var dominantFootList = _dominantFootPositions.ToList();
			var timestampList = _timestamps.ToList();

			if (dominantFootList.Count < 2 || timestampList.Count < 2)
				return Vector3.zero;

			// Step 1: Calculate velocity vectors
			var velocities = new List<Vector3>();
			for (var i = 1; i < dominantFootList.Count; i++)
			{
				var deltaPosition = dominantFootList[i] - dominantFootList[i - 1];
				var deltaTime = timestampList[i] - timestampList[i - 1];

				if (deltaTime > 0)
					velocities.Add(deltaPosition / deltaTime);
				else
					velocities.Add(Vector3.zero);
			}

			// Step 2: Find the highest velocity
			var highestVelocity = Vector3.zero;
			var highestVelocityIndex = -1;

			for (var i = 0; i < velocities.Count; i++)
			{
				if (velocities[i].magnitude > highestVelocity.magnitude)
				{
					highestVelocity = velocities[i];
					highestVelocityIndex = i + 1; // Shift index to match dominantFootList
				}
			}

			// Step 3: Check if there is a timestamp 300ms later
			if (highestVelocityIndex != -1)
			{
				for (var i = highestVelocityIndex; i < timestampList.Count; i++)
				{
					if (timestampList[i] - timestampList[highestVelocityIndex - 1] >= 0.3f)
					{
						highestVelocity = Vector3.zero;
						break;
					}
				}
			}

			return highestVelocity;
		}


		public InputData ToInputData()
		{
			var dominantFootList = _dominantFootPositions.ToList();
			var nonDominantFootList = _nonDominantFootPositions.ToList();
			var timestampList = _timestamps.ToList();

			PadLeft(dominantFootList, Vector3.zero, _maxSize);
			PadLeft(nonDominantFootList, Vector3.zero, _maxSize);
			PadLeft(timestampList, 0f, _maxSize);

			return new InputData(dominantFootList, nonDominantFootList, timestampList);
		}

		void PadLeft<T>(List<T> list, T defaultValue, int targetSize)
		{
			while (list.Count < targetSize)
				list.Insert(0, defaultValue);
		}

		readonly Queue<Vector3> _dominantFootPositions = new();

		readonly int _maxSize;
		readonly Queue<Vector3> _nonDominantFootPositions = new();
		readonly Queue<float> _timestamps = new();
	}
}
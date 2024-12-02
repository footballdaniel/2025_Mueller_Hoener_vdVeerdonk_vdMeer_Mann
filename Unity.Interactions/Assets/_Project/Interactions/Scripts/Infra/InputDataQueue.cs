using System.Collections.Generic;
using System.Linq;
using _Project.PassDetection.Common;
using UnityEngine;

namespace Interactions.Scripts.Infra
{
	public class InputDataQueue
	{
		readonly int _maxSize;

		Queue<Vector3> _dominantFootPositions = new();
		Queue<Vector3> _nonDominantFootPositions = new();
		Queue<float> _timestamps = new();

		public InputDataQueue(int maxSize = 10)
		{
			_maxSize = maxSize;
		}

		public InputData ToInputData()
		{
			// Convert queues to lists
			var dominantFootList = _dominantFootPositions.ToList();
			var nonDominantFootList = _nonDominantFootPositions.ToList();
			var timestampList = _timestamps.ToList();

			// Left pad each list to _maxSize with default values
			PadLeft(dominantFootList, Vector3.zero, _maxSize);
			PadLeft(nonDominantFootList, Vector3.zero, _maxSize);
			PadLeft(timestampList, 0f, _maxSize);

			// Create InputData object
			return new InputData(dominantFootList, nonDominantFootList, timestampList);
		}

		void PadLeft<T>(List<T> list, T defaultValue, int targetSize)
		{
			while (list.Count < targetSize)
				list.Insert(0, defaultValue); // Add default value to the beginning of the list
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
	}
}
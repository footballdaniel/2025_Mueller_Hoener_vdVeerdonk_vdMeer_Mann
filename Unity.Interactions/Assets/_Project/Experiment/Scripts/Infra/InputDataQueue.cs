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

		/// Reports the peak velocity across both feet and which foot produced it, so the
		/// caller can spawn the ball at the foot that actually kicked. Ties go to the
		/// dominant foot.
		// OPTIONAL: If needed, report not the peak but the direction a short delay (300ms)
		// after the peak, by tracking the index of the peak sample in PeakVelocity.
		public KickingFoot CalculateHighestObservedVelocity()
		{
			var timestampList = _timestamps.ToList();
			var dominantVelocity = PeakVelocity(_dominantFootPositions.ToList(), timestampList);
			var nonDominantVelocity = PeakVelocity(_nonDominantFootPositions.ToList(), timestampList);

			return nonDominantVelocity.magnitude > dominantVelocity.magnitude
				? new KickingFoot(nonDominantVelocity, false)
				: new KickingFoot(dominantVelocity, true);
		}

		static Vector3 PeakVelocity(List<Vector3> positions, List<float> timestamps)
		{
			if (positions.Count < 2 || timestamps.Count < 2)
				return Vector3.zero;

			var highestVelocity = Vector3.zero;

			for (var i = 1; i < positions.Count; i++)
			{
				var deltaTime = timestamps[i] - timestamps[i - 1];
				if (deltaTime <= 0)
					continue;

				var velocity = (positions[i] - positions[i - 1]) / deltaTime;
				if (velocity.magnitude > highestVelocity.magnitude)
					highestVelocity = velocity;
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
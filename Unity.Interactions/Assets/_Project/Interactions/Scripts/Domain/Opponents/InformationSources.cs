using System.Collections.Generic;
using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class InformationSources
	{
		public void ActivateOnly(IInformationSource activeSource)
		{
			for (var i = 0; i < _sources.Count; i++)
			{
				var (src, _) = _sources[i];
				_sources[i] = (src, 0f);
			}

			_sources.RemoveAll(x => x.Item1.GetType() == activeSource.GetType());
			_sources.Add((activeSource, 1f));
		}

		public void Add(IInformationSource source, float weight)
		{
			source.Weight = weight;
			_sources.Add((source, weight));
		}

		public Vector3 CombinePositions()
		{
			var desiredPosition = Vector3.zero;
			var totalWeight = 0f;

			foreach (var (source, weight) in _sources)
			{
				var sourceTargetPosition = source.TargetPosition();

				if (sourceTargetPosition.x != 0)
					desiredPosition.x += sourceTargetPosition.x * weight;
				
				if (sourceTargetPosition.y != 0)
					desiredPosition.y += sourceTargetPosition.y * weight;
				
				if (sourceTargetPosition.z != 0)
					desiredPosition.z += sourceTargetPosition.z * weight;

				totalWeight += weight;
			}

			return desiredPosition / totalWeight;
		}

		public float CombineRotationsY()
		{
			var totalWeight = 0f;
			var accumulatedRotation = 0f;

			foreach (var (source, weight) in _sources)
			{
				var targetRotationY = source.TargetRotationY();

				if (targetRotationY != 0)
				{
					accumulatedRotation += source.TargetRotationY() * weight;
					totalWeight += weight;
				}
				
			}

			if (totalWeight > 0f)
				return accumulatedRotation / totalWeight;
			return 0f;
		}

		List<(IInformationSource source, float weight)> _sources = new();
	}
}
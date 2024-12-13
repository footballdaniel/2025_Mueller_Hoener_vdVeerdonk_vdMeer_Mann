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

		public void Update(IInformationSource source, float weight)
		{
			_sources.RemoveAll(x => x.Item1.GetType() == source.GetType());
			
			source.Weight = weight;
			_sources.Add((source, weight));
		}

		public Vector3 CombinePositions()
		{
			var desiredPosition = Vector3.zero;
			var totalWeightX = 0f;
			var totalWeightY = 0f;
			var totalWeightZ = 0f;
			
			

			foreach (var (source, weight) in _sources)
			{
				var sourceTargetPosition = source.TargetPosition();

				if (sourceTargetPosition.x != 0)
				{
					desiredPosition.x += sourceTargetPosition.x * weight;
					totalWeightX += weight;
				}

				if (sourceTargetPosition.y != 0)
				{
					desiredPosition.y += sourceTargetPosition.y * weight;
					totalWeightY += weight;
				}

				if (sourceTargetPosition.z != 0)
				{
					desiredPosition.z += sourceTargetPosition.z * weight;
					totalWeightZ += weight;
				}
			}
			
			var desiredPositionX = totalWeightX > 0 ? desiredPosition.x / totalWeightX : 0;
			var desiredPositionY = totalWeightY > 0 ? desiredPosition.y / totalWeightY : 0;
			var desiredPositionZ = totalWeightZ > 0 ? desiredPosition.z / totalWeightZ : 0;
			
			return new Vector3(desiredPositionX, desiredPositionY, desiredPositionZ);
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
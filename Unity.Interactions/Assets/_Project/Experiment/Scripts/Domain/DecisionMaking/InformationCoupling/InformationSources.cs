using System.Collections.Generic;
using UnityEngine;

namespace Interactions.Domain.DecisionMaking.InformationCoupling
{
	public class InformationSources
	{
		public void ActivateOnly(IInformationSource activeSource)
		{
			for (var i = 0; i < _sources.Count; i++)
			{
				var (src, weight, _) = _sources[i];
				_sources[i] = (src, weight, false);
			}

			_sources.RemoveAll(x => x.Item1.GetType() == activeSource.GetType());
			_sources.Add((activeSource, 1f, true));
		}

		public void Remove(IInformationSource source)
		{
			_sources.RemoveAll(x => x.Item1.GetType() == source.GetType());
		}

		public void ActivateAll()
		{
			for (var i = 0; i < _sources.Count; i++)
			{
				var (src, weight, _) = _sources[i];
				_sources[i] = (src, weight, true);
			}
		}

		public void AddNewSource(IInformationSource source, float weight)
		{
			_sources.RemoveAll(x => x.Item1.GetType() == source.GetType());

			source.Weight = weight;
			_sources.Add((source, weight, true));
		}

		public Vector3 CombinePositions()
		{
			var desiredPosition = Vector3.zero;
			var totalWeightX = 0f;
			var totalWeightY = 0f;
			var totalWeightZ = 0f;

			foreach (var (source, weight, isActive) in _sources)
			{
				if (!isActive)
					continue;

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

			foreach (var (source, weight, _) in _sources)
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

		List<(IInformationSource source, float weight, bool isActive)> _sources = new();
	}
}
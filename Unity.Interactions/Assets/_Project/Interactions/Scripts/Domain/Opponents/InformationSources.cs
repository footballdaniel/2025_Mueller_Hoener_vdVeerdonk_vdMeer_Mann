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
				_sources[i] = (src, src == activeSource ? 1f : 0f);
			}
		}

		public void Add(IInformationSource source, float weight)
		{
			source.Weight = weight;
			_sources.Add((source, weight));
		}

		public Vector3 Combine()
		{
			var desiredPosition = Vector3.zero;
			var totalWeight = 0f;

			foreach (var (source, weight) in _sources)
			{
				desiredPosition += source.GetDesiredPosition() * weight;
				totalWeight += weight;
			}

			return desiredPosition / totalWeight;
		}

		public void SetWeight(IInformationSource source, float weight)
		{
			for (var i = 0; i < _sources.Count; i++)
				if (_sources[i].source == source)
					_sources[i] = (source, weight);
		}

		List<(IInformationSource source, float weight)> _sources = new();
	}
}
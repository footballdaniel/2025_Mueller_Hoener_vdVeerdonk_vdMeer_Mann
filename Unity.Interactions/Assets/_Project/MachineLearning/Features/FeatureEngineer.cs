using System.Collections.Generic;

namespace Tactive.MachineLearning.Features
{
	public class FeatureEngineer<T>
	{
		readonly List<Feature<T>> _features = new();

		public int FeatureSize => _features.Count;

		public void AddFeature(Feature<T> featureCalculator)
		{
			_features.Add(featureCalculator);
		}

		public IReadOnlyList<float> Engineer(T inputData)
		{
			var inputs = new List<float>();

			foreach (var feature in _features)
				inputs.AddRange(feature.Calculate(inputData));

			return inputs;
		}
	}
}
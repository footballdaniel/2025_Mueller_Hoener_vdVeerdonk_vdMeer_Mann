using System.Collections.Generic;

namespace _Project.PassDetection.Replay.Features
{
	public abstract class FeatureCalculator
	{
		public abstract int Size { get; }
		public abstract List<Feature> Calculate(InputData inputData);
	}

	public class Feature
	{

		public Feature(string name, List<float> values)
		{
			Name = name;
			Values = values;
		}

		public string Name { get; }
		public List<float> Values { get; }
	}
}

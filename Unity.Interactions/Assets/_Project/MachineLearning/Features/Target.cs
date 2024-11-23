using System.Collections.Generic;

namespace Tactive.MachineLearning.Features
{
	public class Target
	{

		public Target(string name, List<float> values)
		{
			Name = name;
			Values = values;
		}

		public string Name { get; }
		public List<float> Values { get; }
	}
}
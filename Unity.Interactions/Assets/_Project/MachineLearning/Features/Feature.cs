using System.Collections.Generic;

namespace Tactive.MachineLearning._Project.MachineLearning
{
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


using System.Collections.Generic;

namespace Tactive.MachineLearning._Project.MachineLearning
{
	public abstract class BaseFeature<T>
	{
		public string Name => GetType().Name;
		public abstract int Size { get; }
		public abstract List<Feature> Calculate(T inputData);
	}
}

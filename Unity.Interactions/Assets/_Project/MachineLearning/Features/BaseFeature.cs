

using System.Collections.Generic;
using Tactive.MachineLearning.Features;

namespace Tactive.MachineLearning._Project.MachineLearning
{
	public abstract class BaseFeature<T>
	{
		public string Name => GetType().Name;
		public abstract int Size { get; }
		public abstract List<Target> Calculate(T inputData);
	}
}

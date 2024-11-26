using System.Collections.Generic;

namespace Tactive.MachineLearning.Features
{
	public abstract class BaseFeature<T>
	{
		public string Name => GetType().Name;
		public abstract int Size { get; }
		public abstract List<Target> Calculate(T inputData);
	}
}

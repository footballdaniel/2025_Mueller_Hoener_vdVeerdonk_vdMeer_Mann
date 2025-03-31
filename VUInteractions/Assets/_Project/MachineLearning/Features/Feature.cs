using System.Collections.Generic;


public abstract class Feature<T>
{
	public abstract IReadOnlyList<float> Calculate(T inputData);

	public string Name => GetType().Name;
}
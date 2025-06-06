using System;
using System.Collections.Generic;
using System.Linq;

namespace Tactive.MachineLearning.Features
{
	public static class FeatureRegistry
	{
		readonly static Dictionary<string, Type> _registry = new();

		static FeatureRegistry()
		{
			RegisterAllFeatures();
		}

		public static Feature<T> Create<T>(string featureName)
		{
			if (!_registry.TryGetValue(featureName, out var featureType))
				throw new ArgumentException($"No feature class registered under name {featureName}");

			if (typeof(Feature<T>).IsAssignableFrom(featureType))
				return (Feature<T>)Activator.CreateInstance(featureType);

			throw new InvalidOperationException($"Feature '{featureName}' is not compatible with input type {typeof(T).Name}");
		}

		static void RegisterAllFeatures()
		{
			var featureTypes = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(a => a.GetTypes())
				.Where(t => t.BaseType is { IsGenericType: true } && t.BaseType.GetGenericTypeDefinition() == typeof(Feature<>))
				.ToList();

			foreach (var featureType in featureTypes)
				_registry[featureType.Name] = featureType;
		}
	}
}
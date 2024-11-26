using System;
using System.Collections.Generic;
using System.Linq;

namespace Tactive.MachineLearning.Features
{
	public static class FeatureRegistry
	{
		static FeatureRegistry()
		{
			RegisterAllFeatures();
		}

		public static BaseFeature<T> Create<T>(string featureName)
		{
			if (!_registry.TryGetValue(featureName, out var featureType))
				throw new ArgumentException($"No feature class registered under name {featureName}");

			if (typeof(BaseFeature<T>).IsAssignableFrom(featureType))
				return (BaseFeature<T>)Activator.CreateInstance(featureType);
			
			throw new InvalidOperationException($"Feature '{featureName}' is not compatible with input type {typeof(T).Name}");
		}

		static void RegisterAllFeatures()
		{
			var featureTypes = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(a => a.GetTypes())
				.Where(t => t.BaseType is { IsGenericType: true } && t.BaseType.GetGenericTypeDefinition() == typeof(BaseFeature<>))
				.ToList();

			foreach (var featureType in featureTypes)
				_registry[featureType.Name] = featureType;
		}

		readonly static Dictionary<string, Type> _registry = new();
	}
}
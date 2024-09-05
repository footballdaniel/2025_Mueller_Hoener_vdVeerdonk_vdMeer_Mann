using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Services : MonoBehaviour
{
	static Dictionary<Type, object> _singletonServices = new();
	static Dictionary<Type, Func<object>> _scopedServiceFactories = new();

	public static void AddScoped<TInterface>(Func<TInterface> factory) where TInterface : class
	{
		var serviceType = typeof(TInterface);

		if (!_scopedServiceFactories.ContainsKey(serviceType))
		{
			_scopedServiceFactories[serviceType] = () => factory();
			Debug.Log($"Registered scoped service: {serviceType.Name}");
		}
		else
			Debug.LogWarning($"Service of type {serviceType.Name} is already registered.");
	}

	// Add a MonoBehaviour singleton (existing in the scene)
	public static void AddSingleton<TInterface>(TInterface service) where TInterface : class
	{
		var serviceType = typeof(TInterface);

		if (!_singletonServices.ContainsKey(serviceType))
		{
			_singletonServices[serviceType] = service;
			Debug.Log($"Registered singleton MonoBehaviour service: {serviceType.Name}");
		}
		else
			Debug.LogWarning($"Service of type {serviceType.Name} is already registered.");
	}

	// Add a regular (non-MonoBehaviour) singleton
	public static void AddSingleton<TInterface>(Func<TInterface> factory) where TInterface : class
	{
		var serviceType = typeof(TInterface);

		if (!_singletonServices.ContainsKey(serviceType))
		{
			_singletonServices[serviceType] = factory();
			Debug.Log($"Registered singleton service: {serviceType.Name}");
		}
		else
			Debug.LogWarning($"Service of type {serviceType.Name} is already registered.");
	}

	public static T Build<T>() where T : class
	{
		return Resolve<T>();
	}

	static object Resolve(Type type)
	{
		if (_singletonServices.TryGetValue(type, out var singletonService))
			return singletonService;

		if (_scopedServiceFactories.TryGetValue(type, out var scopedFactory))
			return scopedFactory();

		Debug.LogWarning($"Service of type {type.Name} not found.");
		return null;
	}

	
	static T Resolve<T>() where T : class
	{
		var constructor = typeof(T).GetConstructors().FirstOrDefault();

		if (constructor == null)
			throw new InvalidOperationException($"No constructor found for type {typeof(T).Name}");


		var parameters = constructor.GetParameters();
		// Resolve each parameter by checking the registered services
		var args = new List<object>();

		foreach (var parameter in parameters)
		{
			var resolved = Resolve(parameter.ParameterType);

			if (resolved == null)
				throw new InvalidOperationException($"Cannot resolve parameter {parameter.Name} of type {parameter.ParameterType.Name}");
			args.Add(resolved);
		}

		// Create the instance using the resolved arguments (not for MonoBehaviours)
		if (typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
			throw new InvalidOperationException($"Cannot create a MonoBehaviour using new for {typeof(T).Name}. Use AddSingleton to register MonoBehaviours.");

		return (T)Activator.CreateInstance(typeof(T), args.ToArray());
	}
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
	// Use a dictionary to store services
	static Dictionary<Type, object> _services = new();

	// Retrieve a service by type from the dictionary
	public static TInterface Get<TInterface>() where TInterface : class
	{
		var serviceType = typeof(TInterface);

		if (_services.TryGetValue(serviceType, out var service))
			return service as TInterface;
		Debug.LogWarning($"Service of type {serviceType.Name} not found.");
		return null;
	}

	void OnValidate()
	{
		_services.Clear();

		// Find all components that implement IService<T>
		foreach (Transform child in transform)
		foreach (var component in child.GetComponents<Component>())
		{
			var interfaces = component.GetType().GetInterfaces();

			foreach (var iface in interfaces)
				if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IService<>))
				{
					// Extract the generic argument (T in IService<T>)
					var serviceType = iface.GetGenericArguments()[0];

					// Register the service via the Services class
					Services.AddSingleton(serviceType, component);
				}
		}
	}
}

public static class Services
{
	static Dictionary<Type, object> _singletonServices = new();

	// Add a singleton service with a specific type
	public static void AddSingleton(Type interfaceType, object implementation)
	{
		if (!_singletonServices.ContainsKey(interfaceType))
		{
			_singletonServices[interfaceType] = implementation;
			Debug.Log($"Registered singleton service: {interfaceType.Name}");
		}
		else
			Debug.LogWarning($"Service of type {interfaceType.Name} is already registered.");
	}

	// Generic method to register a singleton service
	public static void AddSingleton<TInterface>(TInterface service) where TInterface : class
	{
		var serviceType = typeof(TInterface);

		if (!_singletonServices.ContainsKey(serviceType))
		{
			_singletonServices[serviceType] = service;
			Debug.Log($"Registered singleton service: {serviceType.Name}");
		}
		else
			Debug.LogWarning($"Service of type {serviceType.Name} is already registered.");
	}

	// Generic method to get a singleton service
	public static TInterface GetService<TInterface>() where TInterface : class
	{
		var serviceType = typeof(TInterface);

		if (_singletonServices.TryGetValue(serviceType, out var service))
			return service as TInterface;
		Debug.LogWarning($"Service of type {serviceType.Name} not found.");
		return null;
	}
}

public interface IService<T>
{
	// Define any required methods for the service
}
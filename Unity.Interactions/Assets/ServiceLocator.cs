using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
	// Dictionary to store services
	private static Dictionary<Type, object> _services = new();

	// Retrieve a service by type from the dictionary
	public static TInterface Get<TInterface>() where TInterface : class
	{
		var serviceType = typeof(TInterface);

		if (_services.TryGetValue(serviceType, out var service))
		{
			return service as TInterface;
		}

		Debug.LogWarning($"Service of type {serviceType.Name} not found.");
		return null;
	}

	// Scan the child objects of the ServiceLocator and register services
	public void DiscoverServices()
	{
		_services.Clear();

		// Find all components in child objects of the ServiceLocator
		foreach (Transform child in transform)
		{
			foreach (var component in child.GetComponents<Component>())
			{
				// Find components that implement IService<T>
				var interfaces = component.GetType().GetInterfaces();
				foreach (var iface in interfaces)
				{
					if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IService<>))
					{
						// Extract the generic argument (T in IService<T>)
						var serviceType = iface.GetGenericArguments()[0];

						// Register the service in the service dictionary
						if (!_services.ContainsKey(serviceType))
						{
							_services.Add(serviceType, component);
							Debug.Log($"Discovered and registered service: {serviceType.Name}");
						}
					}
				}
			}
		}
	}

	// Auto-discover services when the script is validated in the editor
	void OnValidate()
	{
		DiscoverServices();
	}
}

// Generic service interface definition
public interface IService<T>
{
	// Define service-specific methods or properties
}
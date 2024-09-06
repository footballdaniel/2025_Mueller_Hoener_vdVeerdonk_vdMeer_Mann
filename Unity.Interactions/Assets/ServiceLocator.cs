using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
	static Dictionary<Type, object> _services = new();

	public static T Get<T>() where T : class
	{
		_services.TryGetValue(typeof(T), out var service);
		return service as T;
	}

	void OnEnable()
	{
		_services.Clear();

		foreach (var service in GetComponentsInChildren<MonoBehaviour>())
		{
			var serviceType = service.GetType();

			if (!_services.TryAdd(serviceType, service))
				Debug.LogWarning($"Service {serviceType.Name} is already registered");
		}
		
		// log name of sservices
		var serviceNames = string.Join(", ", _services.Keys);
		Debug.Log($"Registered services: {serviceNames}");
	}
}
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

		foreach (var component in GetComponentsInChildren<MonoBehaviour>())
		{
			RegisterMonobehaviours(component);
			RegisterInterfaces(component);
		}
		
		var serviceNames = string.Join(", ", _services.Keys);
		Debug.Log($"Registered services: {serviceNames}");
	}

	#region Implementation

	static void RegisterInterfaces(MonoBehaviour service)
	{
		var serviceType = service.GetType();

		foreach (var interfaceType in serviceType.GetInterfaces())
		{
			if (!_services.TryAdd(interfaceType, service))
				Debug.LogWarning($"Service {interfaceType.Name} is already registered");
		}
	}

	static void RegisterMonobehaviours(MonoBehaviour service)
	{
		var serviceType = service.GetType();

		if (!_services.TryAdd(serviceType, service))
			Debug.LogWarning($"Service {serviceType.Name} is already registered");
	}

	#endregion
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
	[SerializeField] GameObject _services;

	void Start()
	{
		var services = new Dictionary<Type, IService>();

		// iterate over services
		foreach (Transform child in _services.transform)
		{
			var service = child.GetComponent<IService>();
			if (service != null)
			{
				var serviceType = service.GetType();
				if (!services.ContainsKey(serviceType))
				{
					services.Add(serviceType, service);
					Debug.Log($"Registered service: {serviceType.Name}");
				}
			}
		}
	}

	void Update()
	{

	}
}

public interface IService
{
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.App
{
	public class ServiceLocator : MonoBehaviour
	{
		[SerializeField] List<GameObject> _prefabs;
		[SerializeField] List<GameObject> _monobehaviours;

		void OnEnable()
		{
			_services.Clear();

			RegisterObjects(_prefabs);
			RegisterObjects(_monobehaviours);

			RegisterServices(transform);

			var serviceNames = string.Join(", ", _services.Keys);
			Debug.Log($"Registered services: {serviceNames}");
		}

		void RegisterServices(Transform transform1)
		{
			for (var i = 0; i < transform1.childCount; i++)
			{
				var child = transform1.GetChild(i);
				var components = child.GetComponents<Component>();

				foreach (var component in components)
					RegisterComponent(component);
			}
		}

		void RegisterObjects(List<GameObject> objects)
		{
			foreach (var prefab in objects)
			{
				var components = prefab.GetComponents<Component>();

				foreach (var component in components)
					RegisterComponent(component);
			}
		}

		void RegisterComponent(Component component)
		{
			var type = component.GetType();
			_services.TryAdd(type, component);

			var interfaces = type.GetInterfaces();
			foreach (var interfaceType in interfaces)
				if (!_services.ContainsKey(interfaceType)) 
					_services.TryAdd(interfaceType, component);
		}

		public static T Get<T>() where T : class
		{
			_services.TryGetValue(typeof(T), out var service);
			return service as T;
		}

		static Dictionary<Type, Component> _services = new();
		
	}
}

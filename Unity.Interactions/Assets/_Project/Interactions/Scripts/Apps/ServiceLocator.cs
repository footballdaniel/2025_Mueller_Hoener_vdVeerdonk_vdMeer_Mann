using System;
using System.Collections.Generic;
using Interactions.Domain;
using UnityEngine;

namespace Interactions.Apps
{
	[Serializable]
	public class ServiceTypeReference
	{
		public string assemblyQualifiedName;
	}


	public class ServiceLocator : MonoBehaviour
	{
		static Dictionary<Type, object> _services = new();
		[SerializeField] List<GameObject> _prefabsWithInterfaces;
		[SerializeField] List<GameObject> _prefabs;
		[SerializeField] List<GameObject> _monobehaviours;
		[SerializeField] List<ServiceTypeReference> _nonMonoBehaviours;
		[SerializeField] List<ScriptableObject> _scriptableObjects;
		[SerializeField] List<AudioClip> _audioClips;


		void OnEnable()
		{
			_services.Clear();

			RegisterMonoBehavioursAsFactory(_prefabsWithInterfaces);
			RegisterObjects(_prefabs);
			RegisterObjects(_monobehaviours);
			RegisterNonMonobehaviours(_nonMonoBehaviours);
			RegisterServices(transform);
			RegisterScriptableObjects(_scriptableObjects);

			var serviceNames = string.Join(", ", _services.Keys);
			Debug.Log($"Registered services: {serviceNames}");
		}

		void RegisterScriptableObjects(List<ScriptableObject> scriptableObjects)
		{
			foreach (var scriptableObject in scriptableObjects)
			{
				var type = scriptableObject.GetType();
				_services.TryAdd(type, scriptableObject);
			}
		}

		public static T Get<T>() where T : class
		{
			_services.TryGetValue(typeof(T), out var service);
			return service as T;
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


		void RegisterMonoBehavioursAsFactory(List<GameObject> prefabsWithInterfaces)
		{
			foreach (var prefab in prefabsWithInterfaces)
			{
				var components = prefab.GetComponents<Component>();

				foreach (var component in components)
				{
					var interfaceType = component.GetType().GetInterfaces()[0];
					var factoryType = typeof(Factory<>).MakeGenericType(interfaceType);
					var factory = Activator.CreateInstance(factoryType, component);
					_services.TryAdd(interfaceType, factory);
				}
			}
		}

		void RegisterNonMonobehaviours(List<ServiceTypeReference> serviceTypes)
		{
			foreach (var serviceTypeRef in serviceTypes)
			{
				var type = Type.GetType(serviceTypeRef.assemblyQualifiedName);

				if (type == null)
				{
					Debug.LogWarning($"Could not find type {serviceTypeRef.assemblyQualifiedName}");
					continue;
				}

				if (type.GetConstructor(Type.EmptyTypes) == null)
				{
					Debug.LogWarning($"Type {type.Name} does not have a parameterless constructor");
					continue;
				}

				var serviceInstance = Activator.CreateInstance(type);
				_services.TryAdd(type, serviceInstance);
				var interfaces = type.GetInterfaces();

				foreach (var interfaceType in interfaces)
					if (!_services.ContainsKey(interfaceType))
						_services.TryAdd(interfaceType, serviceInstance);
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
	}
}
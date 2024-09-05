using System;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
	[SerializeField] GameObject _services;
	[SerializeField] 

	void Start()
	{
		Services.AddSingleton(ServiceLocator.Get<IWebcamRecorder>());
	}

	void Update()
	{

	}
}

public interface IService
{
}
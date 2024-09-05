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
	Services.AddScoped<IWeatherService>(() => new WeatherService());

	// Build and run the app automatically with DI
	var app = Services.Build<App>();

	// Run the application
	app.Run();
}


	void Update()
	{

	}
}

public interface IService
{
}
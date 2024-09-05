using UnityEngine;

public class App
{
	private readonly IWebcamRecorder _webcamRecorder;
	private readonly IWeatherService _weatherService1;
	private readonly IWeatherService _weatherService2;

	// Inject two different weather services
	public App(IWebcamRecorder webcamRecorder, [Scoped] IWeatherService weatherService1, [Scoped] IWeatherService weatherService2)
	{
		_webcamRecorder = webcamRecorder;
		_weatherService1 = weatherService1;
		_weatherService2 = weatherService2;
	}

	public void Run()
	{
		// Show that the weather services are different
		Debug.Log($"WeatherService1 ID: {_weatherService1.GetId()}");
		Debug.Log($"WeatherService2 ID: {_weatherService2.GetId()}");

		// Use the singleton service
		_webcamRecorder.StartRecording();
		_webcamRecorder.StopRecording();

		// Use the weather services
		Debug.Log($"Weather forecast from service 1: {_weatherService1.GetForecast()}");
		Debug.Log($"Weather forecast from service 2: {_weatherService2.GetForecast()}");
	}
}
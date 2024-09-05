using UnityEngine;

public class App
{
	private readonly IWebcamRecorder _webcamRecorder;
	private readonly IWeatherService _weatherService;

	public App(IWebcamRecorder webcamRecorder, IWeatherService weatherService)
	{
		_webcamRecorder = webcamRecorder;
		_weatherService = weatherService;
	}

	// Simulate the application run
	public void Run()
	{
		// Use the singleton service
		_webcamRecorder.StartRecording();
		_webcamRecorder.StopRecording();

		// Use the scoped service
		var forecast = _weatherService.GetForecast();
		Debug.Log(forecast);
	}
}
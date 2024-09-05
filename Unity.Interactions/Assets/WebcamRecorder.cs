using System;
using UnityEngine;

public interface IWebcamRecorder
{
	void StartRecording();
	void StopRecording();
}

public class WebcamRecorder : MonoBehaviour, IWebcamRecorder
{
	public void StartRecording()
	{
		Debug.Log("Webcam recording started");
	}

	public void StopRecording()
	{
		Debug.Log("Webcam recording stopped");
	}
}

public interface IWeatherService
{
	string GetForecast();
	string GetId(); // To identify the instance
}

public class WeatherService : IWeatherService
{
	private string _id;

	public WeatherService()
	{
		_id = Guid.NewGuid().ToString(); // Unique ID per instance
	}

	public string GetForecast()
	{
		return "Sunny with a chance of rain";
	}

	public string GetId()
	{
		return _id;
	}
}

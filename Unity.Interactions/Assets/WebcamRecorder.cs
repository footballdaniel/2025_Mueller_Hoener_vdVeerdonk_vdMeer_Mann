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
}

public class WeatherService : IWeatherService
{
	public string GetForecast()
	{
		return "Sunny with a chance of rain";
	}
}
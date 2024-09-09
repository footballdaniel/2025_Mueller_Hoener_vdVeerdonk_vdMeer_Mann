using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Windows.WebCam;

/// <summary>
/// Implementation using Unity's VideoCapture API to record webcam video.
/// </summary>
public class WebcamRecorder : MonoBehaviour, IWebcamRecorder
{
	[SerializeField] float _selectedFrameRate;
	[SerializeField] Resolution _selectedResolution;

	string _filePath;
	bool _isRecording;
	bool _stopAlreadyRequested;
	VideoCapture _videoCapture;

	// Public properties to expose selected resolution and frame rate
	public Resolution SelectedResolution
	{
		get => _selectedResolution;
		set => _selectedResolution = value;
	}

	// Public property to get all available resolutions
	public List<Resolution> AvailableResolutions => new(VideoCapture.SupportedResolutions);
	public bool IsRecording => _isRecording;

	public float FrameRate
	{
		get => _selectedFrameRate;
		set => _selectedFrameRate = value;
	}

	// Start recording process
	public void StartRecording()
	{
		if (_isRecording)
		{
			Debug.LogWarning("Already recording.");
			return;
		}

		VideoCapture.CreateAsync(false, OnVideoCaptureCreatedCallback);
	}

	// Stop recording process
	public void StopRecording()
	{
		if (!_isRecording || _stopAlreadyRequested)
			return;

		_stopAlreadyRequested = true;
		_videoCapture?.StopRecordingAsync(OnStoppedRecordingVideoCallback);
	}

	// Method to get available frame rates for a specific resolution
	public List<float> GetAvailableFrameratesFor(Resolution resolution)
	{
		return new List<float>(VideoCapture.GetSupportedFrameRatesForResolution(resolution));
	}

	// Callback when the video capture instance is created
	void OnVideoCaptureCreatedCallback(VideoCapture videoCapture)
	{
		_videoCapture = videoCapture;

		// Ensure valid resolution and frame rate
		var cameraResolution = _selectedResolution;

		var cameraParameters = new CameraParameters
		{
			hologramOpacity = 0.0f,
			frameRate = _selectedFrameRate,
			cameraResolutionWidth = cameraResolution.width,
			cameraResolutionHeight = cameraResolution.height,
			pixelFormat = CapturePixelFormat.BGRA32
		};

		_videoCapture.StartVideoModeAsync(cameraParameters,
			VideoCapture.AudioState.ApplicationAndMicAudio,
			OnStartedVideoCaptureModeCallback);
	}

	// Callback when video mode starts
	void OnStartedVideoCaptureModeCallback(VideoCapture.VideoCaptureResult result)
	{
		if (result.success)
		{
			var timeStamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
			var filename = $"Recording_{timeStamp}.mp4";
			_filePath = Path.Combine(Application.persistentDataPath, filename);

			_videoCapture.StartRecordingAsync(_filePath, OnStartedRecordingVideoCallback);
		}
		else
			Debug.LogError("Failed to start video capture mode.");
	}

	// Callback when recording starts
	void OnStartedRecordingVideoCallback(VideoCapture.VideoCaptureResult result)
	{
		if (result.success)
		{
			Debug.Log("Started recording video.");
			_isRecording = true;
		}
		else
			Debug.LogError("Failed to start recording video.");
	}

	// Callback when recording stops
	void OnStoppedRecordingVideoCallback(VideoCapture.VideoCaptureResult result)
	{
		if (result.success)
		{
			Debug.Log($"Stopped recording video. File saved to: {_filePath}");
			_isRecording = false;
			_stopAlreadyRequested = false;
			_videoCapture?.StopVideoModeAsync(OnStoppedVideoCaptureModeCallback);
		}
		else
			Debug.LogError("Failed to stop recording video.");
	}

	// Callback when video mode stops
	void OnStoppedVideoCaptureModeCallback(VideoCapture.VideoCaptureResult result)
	{
		if (result.success)
			Debug.Log("Stopped video capture mode.");
		else
			Debug.LogError("Failed to stop video capture mode.");

		// Dispose the VideoCapture object
		_videoCapture?.Dispose();
		_videoCapture = null;
	}
}
using UnityEngine;
using UnityEngine.Windows.WebCam;
using System;
using System.Collections.Generic;

public class VideoCaptureRecorder : MonoBehaviour
{
    public bool IsRecording => _isRecording;

    private VideoCapture _videoCapture = null;
    private bool _isRecording = false;
    private string _filePath;

    [SerializeField] private Resolution _selectedResolution;
    [SerializeField] private float _selectedFrameRate;
    private bool _stopAlreadyRequested;

    // Public properties to expose selected resolution and frame rate
    public Resolution SelectedResolution
    {
        get => _selectedResolution;
        set => _selectedResolution = value;
    }

    public float FrameRate
    {
        get => _selectedFrameRate;
        set => _selectedFrameRate = value;
    }

    // Public property to get all available resolutions
    public List<Resolution> AvailableResolutions => new List<Resolution>(VideoCapture.SupportedResolutions);

    // Method to get available frame rates for a specific resolution
    public List<float> GetAvailableFrameratesFor(Resolution resolution)
    {
        return new List<float>(VideoCapture.GetSupportedFrameRatesForResolution(resolution));
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

    // Callback when the video capture instance is created
    private void OnVideoCaptureCreatedCallback(VideoCapture videoCapture)
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
    private void OnStartedVideoCaptureModeCallback(VideoCapture.VideoCaptureResult result)
    {
        if (result.success)
        {
            var timeStamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var filename = $"Recording_{timeStamp}.mp4";
            _filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);

            _videoCapture.StartRecordingAsync(_filePath, OnStartedRecordingVideoCallback);
        }
        else
        {
            Debug.LogError("Failed to start video capture mode.");
        }
    }

    // Callback when recording starts
    private void OnStartedRecordingVideoCallback(VideoCapture.VideoCaptureResult result)
    {
        if (result.success)
        {
            Debug.Log("Started recording video.");
            _isRecording = true;
        }
        else
        {
            Debug.LogError("Failed to start recording video.");
        }
    }

    // Callback when recording stops
    private void OnStoppedRecordingVideoCallback(VideoCapture.VideoCaptureResult result)
    {
        if (result.success)
        {
            Debug.Log($"Stopped recording video. File saved to: {_filePath}");
            _isRecording = false;
            _stopAlreadyRequested = false;
            _videoCapture?.StopVideoModeAsync(OnStoppedVideoCaptureModeCallback);
        }
        else
        {
            Debug.LogError("Failed to stop recording video.");
        }
    }

    // Callback when video mode stops
    private void OnStoppedVideoCaptureModeCallback(VideoCapture.VideoCaptureResult result)
    {
        if (result.success)
        {
            Debug.Log("Stopped video capture mode.");
        }
        else
        {
            Debug.LogError("Failed to stop video capture mode.");
        }

        // Dispose the VideoCapture object
        _videoCapture?.Dispose();
        _videoCapture = null;
    }
}

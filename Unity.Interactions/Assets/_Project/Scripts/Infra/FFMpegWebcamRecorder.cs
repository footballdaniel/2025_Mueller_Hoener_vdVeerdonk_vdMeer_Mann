using System;
using System.IO;
using Domain.VideoRecorder;
using UnityEngine;

public class FFMpegWebcamRecorder : IWebcamRecorder
{
	public FFMpegWebcamRecorder(string deviceName, int width, int height, IProgress<int> progress)
	{
		_frameRate = 10;
		Info = new WebcamInfo(deviceName, width, height);
		_progress = progress;
	}

	public bool IsRecording => _webcamTexture is { isPlaying: true } && _isrecording;
	public bool IsExportComplete => _isExportComplete;

	public void StopRecording()
	{
		_isrecording = false;
		_webcamTexture.Stop();
	}

	public void Export()
	{
		var videoOutputPath = Path.Combine(Application.persistentDataPath, "output_video.mp4");
		

		FFMpegExporter.ExportCompleted += OnExportCompleted;
		FFMpegExporter.Export(_frameFolderPath, videoOutputPath, _frameRate, _frameIndex, _progress);
	}

	public WebcamInfo Info { get; private set; }

	public void StartRecording()
	{
		_webcamTexture = new WebCamTexture(Info.DeviceName, Info.Width, Info.Height, 10);
		_webcamTexture.Play();
		_frameFolderPath = Path.Combine(Application.persistentDataPath, "CapturedFrames");

		if (!Directory.Exists(_frameFolderPath))
			Directory.CreateDirectory(_frameFolderPath);

		_isrecording = true;
		_startTime = Time.time;
	}

	public void Tick()
	{
		if (!IsRecording) return;

		var deltaTime = 1f / _frameRate;
		_updateTimer += Time.fixedDeltaTime;
		var epsilon = 0.0001f;

		if (_updateTimer >= deltaTime - epsilon)
		{
			_updateTimer -= deltaTime;
			_frameIndex++;
			SaveFrameAsPng();
		}
	}

	void OnExportCompleted()
	{
		Debug.Log("Export complete!");
		_isExportComplete = true;
		FFMpegExporter.ExportCompleted -= OnExportCompleted;
	}

	void SaveFrameAsPng()
	{
		var tex = new Texture2D(_webcamTexture.width, _webcamTexture.height);
		tex.SetPixels32(_webcamTexture.GetPixels32());
		tex.Apply();

		var frameBytes = tex.EncodeToPNG();
		var fileName = Path.Combine(_frameFolderPath, $"frame_{_frameIndex:D6}.png");
		File.WriteAllBytes(fileName, frameBytes);
	}

	string _frameFolderPath;

	int _frameIndex = 0;
	int _frameRate;
	bool _isExportComplete;
	bool _isrecording;
	float _startTime;
	float _updateTimer;
	WebCamTexture _webcamTexture;
	IProgress<int> _progress;
}
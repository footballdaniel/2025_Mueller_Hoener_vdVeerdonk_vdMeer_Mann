using System;
using System.IO;
using Domain.VideoRecorder;
using UnityEngine;

namespace Infra
{
	public class FFMpegWebcamRecorder : IWebcamRecorder
	{
		public Texture2D Frame => _texture2D;
		
		
		string _frameFolderPath;
		int _frameIndex = 0;
		int _frameRate;
		bool _isExportComplete;
		bool _isrecording;
		IProgress<int> _progress;
		float _startTime;
		Texture2D _texture2D;
		float _updateTimer;
		WebCamTexture _webcamTexture;

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

		public void Export(int trialNumber)
		{
			var fileNameWithDateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
			var fileName = $"trial_{trialNumber}_{fileNameWithDateTime}.mp4";
			var videoOutputPath = Path.Combine(Application.persistentDataPath, fileName);


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
			_isExportComplete = true;
			FFMpegExporter.ExportCompleted -= OnExportCompleted;
		}

		void SaveFrameAsPng()
		{
			_texture2D = new Texture2D(_webcamTexture.width, _webcamTexture.height);
			_texture2D.SetPixels32(_webcamTexture.GetPixels32());
			_texture2D.Apply();

			var frameBytes = _texture2D.EncodeToPNG();
			var fileName = Path.Combine(_frameFolderPath, $"frame_{_frameIndex:D6}.png");
			File.WriteAllBytes(fileName, frameBytes);
		}
	}
}
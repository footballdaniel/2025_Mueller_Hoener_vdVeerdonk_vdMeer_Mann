using System;
using System.IO;
using Interactions.Domain.VideoRecorder;
using UnityEngine;

namespace Interactions.Infra
{
	public class FFMpegWebcamRecorder : IWebcamRecorder
	{

		public FFMpegWebcamRecorder(string deviceName, int width, int height,  IProgress<int> progress)
		{
			Info = new WebcamInfo(deviceName, width, height);
			_progress = progress;
			_exportFramerate = 10f;

			_frameFolderPath = Path.Combine(UnityEngine.Application.persistentDataPath, "CapturedFrames");
		}

		public Texture2D Frame => _texture2D;
		public bool IsExportComplete => _isExportComplete;

		public void StopRecording()
		{
			_webcamTexture.Stop();
		}

		public void Export(int trialNumber)
		{
			var fileNameWithDateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
			var fileName = $"trial_{trialNumber}_{fileNameWithDateTime}.mp4";
			var videoOutputPath = Path.Combine(UnityEngine.Application.persistentDataPath, fileName);


			FFMpegExporter.ExportCompleted += OnExportCompleted;
			FFMpegExporter.Export(_frameFolderPath, videoOutputPath, _exportFramerate, _frameIndex, _progress);
		}

		public void RecordWith(float appRecordingFrameRateHz)
		{
			_exportFramerate = appRecordingFrameRateHz;
		}

		public WebcamInfo Info { get; private set; }

		public void StartRecording()
		{
			_frameIndex = 0;
			
			if (Directory.Exists(_frameFolderPath))
				Directory.Delete(_frameFolderPath, true);
			
			Directory.CreateDirectory(_frameFolderPath);
			
			
			_isExportComplete = false;
			_webcamTexture = new WebCamTexture(Info.DeviceName, Info.Width, Info.Height, (int)_exportFramerate);
			_webcamTexture.Play();
		}

		public void Tick()
		{
			SaveFrameAsPng();
			_frameIndex++;
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

		float _exportFramerate;
		readonly string _frameFolderPath;
		readonly IProgress<int> _progress;
		int _frameIndex;
		bool _isExportComplete;
		Texture2D _texture2D;
		WebCamTexture _webcamTexture;
	}
}
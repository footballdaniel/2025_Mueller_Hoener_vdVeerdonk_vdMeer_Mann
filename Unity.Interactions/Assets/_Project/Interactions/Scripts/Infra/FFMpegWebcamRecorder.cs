using System;
using System.IO;
using Interactions.Domain.VideoRecorder;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Interactions.Infra
{
	public class FfMpegWebcamRecorder : IWebcamRecorder
	{

		public FfMpegWebcamRecorder(WebcamSpecs specs, IProgress<int> progress)
		{
			Specs = specs;
			_progress = progress;
			_frameFolderPath = Path.Combine(UnityEngine.Application.persistentDataPath, "CapturedFrames");
		}

		public Texture2D Frame => _texture2D;
		public bool IsExportComplete => _isExportComplete;
		public bool IsPlaying => _webcamTexture.isPlaying;


		public void StopRecording()
		{
			_isRecording = false;
		}

		public void Export(int trialNumber)
		{
			_isExportComplete = false;
			var fileNameWithDateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
			var fileName = $"trial_{trialNumber}_{fileNameWithDateTime}.mp4";
			var videoOutputPath = Path.Combine(UnityEngine.Application.persistentDataPath, fileName);

			FFMpegExporter.ExportCompleted += OnExportCompleted;
			FFMpegExporter.Export(_frameFolderPath, videoOutputPath, Specs.FrameRate, _frameIndex, _progress);
		}

		public void Initiate()
		{
			_texture2D = new Texture2D(Specs.Width, Specs.Height, TextureFormat.RGBA32, true);
			_webcamTexture = new WebCamTexture(Specs.DeviceName, Specs.Width, Specs.Height, Specs.FrameRate);
			_webcamTexture.Play();
			
			_renderTexture = new RenderTexture(Specs.Width, Specs.Height, 0, RenderTextureFormat.ARGB32);
			_renderTexture.enableRandomWrite = true;
			_renderTexture.Create();

			if (Directory.Exists(_frameFolderPath))
				Directory.Delete(_frameFolderPath, true);

			Directory.CreateDirectory(_frameFolderPath);
		}

		public WebcamSpecs Specs { get; private set; }

		public void StartRecording()
		{
			_isRecording = true;
			_frameIndex = 0;
		}

		public void Tick()
		{
			// TickSynchronous();

			// try
			// {
			// 	Task.Run(TickSynchronous);
			// }
			// catch (Exception e)
			// {
			// 	Debug.LogError(e);
			// }


			
			Graphics.Blit(_webcamTexture, _renderTexture);

			// AsyncGPUReadback.Request(_webcamTexture, 0, Callback);
			AsyncGPUReadback.Request(_renderTexture, 0, GraphicsFormat.R8G8B8A8_UNorm, Callback);
		}

		void Callback(AsyncGPUReadbackRequest request)
		{
			var rawData = request.GetData<Color32>().ToArray();
			_texture2D.SetPixels32(rawData);
			_texture2D.Apply();

			
			if (_isRecording)
				SaveFrameAsPng();
		}
		

		void OnExportCompleted()
		{
			_isExportComplete = true;
			FFMpegExporter.ExportCompleted -= OnExportCompleted;
		}

		void SaveFrameAsPng()
		{
			var frameBytes = _texture2D.EncodeToPNG();
			var fileName = Path.Combine(_frameFolderPath, $"frame_{_frameIndex:D6}.png");
			File.WriteAllBytes(fileName, frameBytes);
			_frameIndex++;
		}

		void TickSynchronous()
		{
			UpdateTexture();

			if (_isRecording)
				SaveFrameAsPng();
		}

		void UpdateTexture()
		{
			_texture2D.SetPixels32(_webcamTexture.GetPixels32());
			_texture2D.Apply(false);
		}


		readonly string _frameFolderPath;
		readonly IProgress<int> _progress;

		float _exportFramerate;
		int _frameIndex;
		bool _isExportComplete;
		bool _isRecording;
		Texture2D _texture2D;
		WebCamTexture _webcamTexture;
		RenderTexture _renderTexture;

		~FfMpegWebcamRecorder()
		{
			_webcamTexture?.Stop();
		}
	}
}
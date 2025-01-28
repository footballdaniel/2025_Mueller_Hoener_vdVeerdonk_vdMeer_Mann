using System;
using System.IO;
using Interactions.Apps;
using Interactions.Domain.VideoRecorders;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Interactions.Infra
{
	public class FfMpegWebcamRecorder : IWebcamRecorder
	{

		public FfMpegWebcamRecorder(WebcamSpecs specs)
		{
			Specs = specs;
		}

		public RenderTexture Frame => _renderTexture;
		public bool IsPlaying => _webcamTexture.isPlaying;

		public WebcamSpecs Specs { get; private set; }


		public void Initiate()
		{
			_webcamTexture = new WebCamTexture(Specs.DeviceName, Specs.Width, Specs.Height, Specs.FrameRate);
			_webcamTexture.Play();

			_renderTexture = new RenderTexture(Specs.Width, Specs.Height, 0, RenderTextureFormat.ARGB32);
			_renderTexture.enableRandomWrite = false;
			_renderTexture.Create();
		}

		public void StartRecording(int currentTrialTrialNumber, ExperimentalCondition condition)
		{
			if (_isRecording) 
				return;
			
			_isRecording = true;

			var fileNameWithDateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
			var path = Path.Combine(Application.persistentDataPath, $"trial_{currentTrialTrialNumber}_{condition.ToString()}_{fileNameWithDateTime}.avi");
			FFMpegExporter.StartExport(path, Specs.Width, Specs.Height, Specs.FrameRate);
		}

		public void StopRecording()
		{
			if (!_isRecording) 
				return;
			
			_isRecording = false;
			FFMpegExporter.EndExport();
		}

		public void Tick()
		{
			Graphics.Blit(_webcamTexture, _renderTexture);
			AsyncGPUReadback.Request(_renderTexture, 0, GraphicsFormat.R8G8B8A8_UNorm, Callback);
		}

		void Callback(AsyncGPUReadbackRequest request)
		{
			if (request.hasError)
			{
				Debug.LogError("GPU readback failed.");
				return;
			}

			var rawDataBytes = request.GetData<byte>().ToArray();

			if (_isRecording)
				FFMpegExporter.WriteFrame(rawDataBytes);
		}

		readonly string _frameFolderPath;
		bool _isRecording;
		RenderTexture _renderTexture;
		WebCamTexture _webcamTexture;

		~FfMpegWebcamRecorder()
		{
			_webcamTexture?.Stop();
		}
	}
}
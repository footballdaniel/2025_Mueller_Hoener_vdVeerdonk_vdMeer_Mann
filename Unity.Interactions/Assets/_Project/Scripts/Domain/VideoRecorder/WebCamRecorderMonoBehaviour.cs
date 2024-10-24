using Infra;
using UnityEngine;

namespace Domain.VideoRecorder
{
	public class WebCamRecorderMonoBehaviour : MonoBehaviour, IWebcamRecorder
	{
		FFMpegWebcamRecorder _recorder;

		public void Set(FFMpegWebcamRecorder recorder)
		{
			_recorder = recorder;
		}

		public Texture2D Frame => _recorder.Frame;
		public bool IsRecording => _recorder.IsRecording;
		public bool IsExportComplete => _recorder.IsExportComplete;
		public void StartRecording()
		{
			_recorder.StartRecording();
		}

		public void FixedUpdate()
		{
			_recorder.Tick();
		}

		public void StopRecording()
		{
			_recorder.StopRecording();
		}

		public void Export()
		{
			_recorder.Export();
		}

		public WebcamInfo Info => _recorder.Info;
	}
}
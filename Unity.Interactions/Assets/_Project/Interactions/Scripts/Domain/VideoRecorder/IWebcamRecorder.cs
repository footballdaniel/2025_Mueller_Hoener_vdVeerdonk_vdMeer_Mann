using UnityEngine;

namespace Interactions.Domain.VideoRecorder
{
	public interface IWebcamRecorder
	{
		Texture2D Frame { get; }
		bool IsExportComplete { get; }
		public WebcamInfo Info { get; }
		void Tick();
		void StartRecording();
		void StopRecording();
		void Export(int trialNumber);
		void RecordWith(float appRecordingFrameRateHz);
	}

	public record WebcamInfo(string DeviceName, int Width, int Height);
}
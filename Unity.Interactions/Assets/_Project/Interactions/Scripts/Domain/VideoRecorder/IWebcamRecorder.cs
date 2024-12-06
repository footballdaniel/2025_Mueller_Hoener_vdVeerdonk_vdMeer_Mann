using UnityEngine;

namespace Interactions.Domain.VideoRecorder
{
	public interface IWebcamRecorder
	{
		Texture2D Frame { get; }
		bool IsExportComplete { get; }
		public WebcamSpecs Specs { get; }
		void Tick();
		void StartRecording();
		void StopRecording();
		void Export(int trialNumber);
		void RecordWith(float appRecordingFrameRateHz);
	}
}
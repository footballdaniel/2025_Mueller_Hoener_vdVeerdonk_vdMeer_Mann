using UnityEngine;

namespace Interactions.Domain.VideoRecorder
{
	public class NoWebcamRecorder : IWebcamRecorder
	{
		public Texture2D Frame => null;
		public bool IsExportComplete => true;
		public WebcamSpecs Specs => new WebcamSpecs("No Recorder", 0, 0);

		public void Tick() { }
		public void StartRecording() { }
		public void StopRecording() { }
		public void Export(int trialNumber) { }
		public void RecordWith(float appRecordingFrameRateHz) { }
	}
}
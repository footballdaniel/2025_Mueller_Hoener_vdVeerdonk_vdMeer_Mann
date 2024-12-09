using UnityEngine;

namespace Interactions.Domain.VideoRecorder
{
	public class NoWebcamRecorder : IWebcamRecorder
	{
		public Texture2D Frame => null;
		public bool IsExportComplete => true;
		public WebcamSpecs Specs => new("No Recorder", 0, 0, 0);
		public bool IsPlaying => true;

		public void Tick()
		{
		}
		
		public void StartRecording()
		{
		}

		public void StopRecording()
		{
		}

		public void Export(int trialNumber)
		{
		}

		public void Initiate()
		{
		}
	}
}
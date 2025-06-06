using Interactions.Apps;
using UnityEngine;

namespace Interactions.Domain.VideoRecorders
{
	public class NoWebcamRecorder : IWebcamRecorder
	{
		public bool IsExportComplete => true;
		public RenderTexture Frame => null;
		public WebcamSpecs Specs => new("No Recorder", 0, 0, 0);
		public bool IsPlaying => true;

		public void Tick()
		{
		}

		public void StartRecording(int currentTrialTrialNumber, ExperimentalCondition condition)
		{
		}

		public void StopRecording()
		{
		}


		public void Initiate()
		{
		}
	}
}
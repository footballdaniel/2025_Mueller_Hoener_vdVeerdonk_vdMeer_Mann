using Interactions.Apps;
using Interactions.Domain.VideoRecorders;
using UnityEngine;

namespace Interactions.Infra
{
	public class NotSpecifiedWebcamRecorder : IWebcamRecorder
	{
		public bool IsExportComplete => true;
		public RenderTexture Frame => null;
		public WebcamSpecs Specs => new("No Recorder", 0, 0, 0);
		public bool IsPlaying => false;

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
using Interactions.Domain.VideoRecorder;
using UnityEngine;

namespace Interactions.Infra
{
	public class NotInitiatedRecorder : IWebcamRecorder
	{
		public Texture2D Frame { get; } = null;
		public bool IsExportComplete { get; } = true;
		public WebcamSpecs Specs { get; } = new("Not initiated recorder", 0, 0, 0);
		public bool IsPlaying { get; } = false;
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
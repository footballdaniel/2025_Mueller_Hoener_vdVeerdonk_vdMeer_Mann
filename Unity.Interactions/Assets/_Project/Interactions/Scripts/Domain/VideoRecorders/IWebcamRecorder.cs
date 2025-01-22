using UnityEngine;

namespace Interactions.Domain.VideoRecorders
{
	public interface IWebcamRecorder
	{
		RenderTexture Frame { get; }
		public WebcamSpecs Specs { get; }
		bool IsPlaying { get; }
		void Tick();
		void StartRecording(int currentTrialTrialNumber);
		void StopRecording();
		void Initiate();
	}
}
namespace Domain.VideoRecorder
{
	public interface IWebcamRecorder
	{
		bool IsRecording { get; }
	
		void InitiateRecorder();
		void StopRecording();
	
	}
}



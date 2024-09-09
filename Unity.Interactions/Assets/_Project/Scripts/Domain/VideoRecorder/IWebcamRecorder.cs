public interface IWebcamRecorder
{
	bool IsRecording { get; }
	
	void InitiateRecorder();
	void StopRecording();
	
}
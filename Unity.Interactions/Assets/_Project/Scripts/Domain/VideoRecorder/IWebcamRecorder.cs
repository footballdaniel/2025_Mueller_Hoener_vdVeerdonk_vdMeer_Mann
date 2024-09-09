public interface IWebcamRecorder
{
	bool IsRecording { get; }
	float FrameRate { get;}
	
	void InitiateRecorder();
	void StopRecording();
	
}
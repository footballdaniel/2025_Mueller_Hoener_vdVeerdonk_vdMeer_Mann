public interface IWebcamRecorder
{
	bool IsRecording { get; }
	float FrameRate { get; set; }
	
	void StartRecording();
	void StopRecording();
	
}
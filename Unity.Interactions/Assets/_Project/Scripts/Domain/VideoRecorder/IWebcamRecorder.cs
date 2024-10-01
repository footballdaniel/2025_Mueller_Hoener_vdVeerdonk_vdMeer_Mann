namespace Domain.VideoRecorder
{
	public interface IWebcamRecorder
	{
		bool IsRecording { get; }
		bool IsExportComplete { get; }
		void StartRecording();
		void StopRecording();
		void Export();
	}
}
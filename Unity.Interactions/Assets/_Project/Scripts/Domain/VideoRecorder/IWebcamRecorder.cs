namespace Domain.VideoRecorder
{
	public interface IWebcamRecorder
	{
		bool IsRecording { get; }
		bool IsExportComplete { get; }
		void StartRecording();
		void StopRecording();
		void Export();
		
		public WebcamInfo Info { get; }
	}

	public record WebcamInfo(string DeviceName, int Width, int Height);
}
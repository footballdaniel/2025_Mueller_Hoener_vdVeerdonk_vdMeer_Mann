namespace Domain.VideoRecorder
{
	public record WebCamConfiguration(string DeviceName, int Width, int Height, int FrameRate);
}
using UnityEngine;

public class NoWebcamRecorder : MonoBehaviour, IWebcamRecorder
{
	public bool IsRecording => true;
	public float FrameRate { get; set; } = 10f;
	
	public void StartRecording()
	{
	}
	
	public void StopRecording()
	{
	}
}
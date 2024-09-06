using System;
using UnityEngine;

public class Game : MonoBehaviour
{
	[SerializeField] private bool _recordWebam;
	[SerializeField] private int _recordingFrameRateHz = 10;

	public VideoCaptureRecorder WebcamRecorder { get; private set; }
	
	
	void Start()
	{
		WebcamRecorder = ServiceLocator.Get<VideoCaptureRecorder>();
		WebcamRecorder.StartRecording();
		
		SetRecordingFrameRate(_recordingFrameRateHz);
	}

	private void SetRecordingFrameRate(int recordingFrameRateHz)
	{
		Time.fixedDeltaTime = 1f / recordingFrameRateHz;
	}

	private void Update()
	{
		if (Time.timeSinceLevelLoad > 3)
		{
			WebcamRecorder.StopRecording();
		}
	}
}
using UnityEngine;

public class Game : MonoBehaviour
{
	[SerializeField] private bool _recordWebam;
	[SerializeField] private int _recordingFrameRateHz = 10;

	public WebcamRecorder WebcamRecorder { get; private set; }
	
	
	void Start()
	{
		WebcamRecorder = ServiceLocator.Get<WebcamRecorder>();
		SetRecordingFrameRate(_recordingFrameRateHz);
	}

	private void SetRecordingFrameRate(int recordingFrameRateHz)
	{
		Time.fixedDeltaTime = 1f / recordingFrameRateHz;
	}


	void FixedUpdate()
	{
		if (_recordWebam)
			WebcamRecorder.RecordFrame();
	}
	
	
}
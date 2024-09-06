using UnityEngine;

public class Game : MonoBehaviour
{
	[Header("Settings"), SerializeField] private bool _recordVideo;
	

	public VideoCaptureRecorder WebcamRecorder { get; private set; }
	
	
	void Start()
	{
		// Get services for app
		WebcamRecorder = ServiceLocator.Get<VideoCaptureRecorder>();
		
		// Setup app behaviour
		var init = new InitState(this);
		var startRecording = new StartRecordingState(this);
		var trial = new TrialState(this);
		var stopRecording = new StopRecordingState(this);
		var end = new EndState(this);

		init.AddTransition(new Transition(startRecording, _recordVideo));
		init.AddTransition(new Transition(trial, !_recordVideo));
		
		trial.AddTransition(new Transition(stopRecording, _recordVideo));
		trial.AddTransition(new Transition(end, !_recordVideo));
		
		stopRecording.AddTransition(new Transition(end, true));
		
		// Start app
		_stateMachine = new StateMachine();
		_stateMachine.SetState(init);
	}



	private StateMachine _stateMachine;
	private void Update()
	{
		_stateMachine.Tick();
	}
}
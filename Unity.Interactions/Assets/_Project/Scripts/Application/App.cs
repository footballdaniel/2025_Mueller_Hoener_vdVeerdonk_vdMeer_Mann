using UnityEngine;

public class App : MonoBehaviour
{
	[Header("Settings"), SerializeField] bool _recordVideo;

	StateMachine _stateMachine;

	public IWebcamRecorder WebcamRecorder { get; private set; }
	public Trial Trial;

	void Start()
	{
		// Get services for app
		WebcamRecorder = ServiceLocator.Get<IWebcamRecorder>();
		
		// Setup app behaviour
		var init = new InitState(this);
		var startRecording = new StartRecordingVideoState(this);
		var trial = new TrialState(this);
		var end = new EndState(this);

		// Adding transitions based on predicates
		init.AddTransition(new Transition(startRecording, new BooleanPredicate(_recordVideo)));
		init.AddTransition(new Transition(trial, new BooleanPredicate(!_recordVideo)));
		
		startRecording.AddTransition(new Transition(trial, new EventPredicate(ref AppEvents.RecordingStarted)));
		
		trial.AddTransition(new Transition(end, new EventPredicate(ref AppEvents.TrialEnded)));

		// Start app
		_stateMachine = new StateMachine();
		_stateMachine.SetState(init);
	}

	void Update()
	{
		_stateMachine.Tick();
	}
}
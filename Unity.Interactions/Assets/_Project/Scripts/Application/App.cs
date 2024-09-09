using UnityEngine;

public class App : MonoBehaviour
{
	[Header("Settings")]
	[field:SerializeReference] public bool RecordVideo { get; private set; }
	
	[Header("Prefabs")]
	public Opponent OpponentPrefab;
	public User User;
	
	
	[Header("Internal state")]
	public IWebcamRecorder WebcamRecorder { get; private set; }
	public Trial Trial { get; set; }
	public Opponent Opponent { get; set; }

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
		init.AddTransition(new Transition(startRecording, new BooleanPredicate(RecordVideo)));
		init.AddTransition(new Transition(trial, new BooleanPredicate(!RecordVideo)));
		startRecording.AddTransition(new Transition(trial, new EventPredicate(ref AppEvents.RecordingStarted)));
		trial.AddTransition(new Transition(end, new EventPredicate(ref AppEvents.TrialEnded)));
		end.AddTransition(new Transition(init, new EventPredicate(ref AppEvents.RestartRequested)));

		// Start app
		_stateMachine = new StateMachine();
		_stateMachine.SetState(init);
	}

	void Update()
	{
		_stateMachine.Tick();
	}
	
	StateMachine _stateMachine;
}
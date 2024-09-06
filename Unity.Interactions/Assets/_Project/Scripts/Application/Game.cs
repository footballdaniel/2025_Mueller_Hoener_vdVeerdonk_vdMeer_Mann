using UnityEngine;

public class Game : MonoBehaviour
{
    [Header("Settings"), SerializeField] private bool _recordVideo;

    public VideoCaptureRecorder WebcamRecorder { get; private set; }
    
    private StateMachine _stateMachine;

    void Start()
    {
        // Get services for app
        WebcamRecorder = ServiceLocator.Get<VideoCaptureRecorder>();

        // Setup app behaviour
        var init = new InitState(this);
        var startRecording = new StartRecordingState(this);
        var trial = new TrialState(this);
        var end = new EndState(this);

        // Adding transitions based on predicates
        init.AddTransition(new Transition(startRecording, new BooleanPredicate(_recordVideo)));
        init.AddTransition(new Transition(trial, new BooleanPredicate(!_recordVideo)));

        // Using EventPredicate to subscribe to GameEvents.TrialEnded
        trial.AddTransition(new Transition(end, new EventPredicate(GameEvents.TrialEnded)));

        // Start app
        _stateMachine = new StateMachine();
        _stateMachine.SetState(init);
    }

    private void Update()
    {
        _stateMachine.Tick();
    }
}
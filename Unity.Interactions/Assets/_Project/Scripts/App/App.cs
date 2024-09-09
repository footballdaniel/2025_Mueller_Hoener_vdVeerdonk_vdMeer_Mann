using _Project.Scripts.App.States;
using UnityEngine;

namespace _Project.Scripts.App
{
	public class App : MonoBehaviour
	{
		[Header("Dependencies")]
		[field: SerializeReference] public UI UI { get; private set; }
	
		[Header("Prefabs")]
		public Opponent OpponentPrefab;
		public User User;
	
		[Header("Settings")]
		[field:SerializeReference] public bool RecordVideo { get; private set; }
	
		[Header("Internal state")]
		public IWebcamRecorder WebcamRecorder { get; private set; }
		public Experiment Experiment { get; private set; }
		public Trial CurrentTrial { get; set; }
		public Opponent Opponent { get; set; }

		void Start()
		{
			// Get services for app
			WebcamRecorder = ServiceLocator.Get<IWebcamRecorder>();

			Experiment = new Experiment();
		
			// Setup app behaviour
			var init = new InitState(this);
			var load = new LoadState(this);
			var startRecording = new StartRecordingVideoState(this);
			var trial = new TrialState(this);
			var end = new EndState(this);

			// Adding transitions based on predicates
			init.AddTransition(new Transition(startRecording, new CompositePredicate(RecordVideo, ref Events.NextTrialRequested)));
			init.AddTransition(new Transition(trial, new CompositePredicate(!RecordVideo, ref Events.NextTrialRequested)));
			startRecording.AddTransition(new Transition(trial, new EventPredicate(ref Events.RecordingStarted)));
			trial.AddTransition(new Transition(end, new EventPredicate(ref Events.TrialEnded)));
			end.AddTransition(new Transition(init, new EventPredicate(ref Events.NextTrialRequested)));

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

	internal class LoadState : State
	{
		public LoadState(App app) : base(app)
		{
			
		}

		public override void Enter()
		{
			
		}
	}
}
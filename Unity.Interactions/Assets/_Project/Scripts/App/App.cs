using System.Runtime.CompilerServices;
using _Project.Scripts.App.States;
using UnityEngine;

namespace _Project.Scripts.App
{
	public class Transitions
	{
		public Transition EndTrial { get; set; }
		public Transition StartRecording;
		public Transition StartTrial;
	}


	public class Transition
	{

		public Transition(App app, State from, State to)
		{
			_stateMachine = app.StateMachine;
			_from = from;
			_to = to;
		}

		public void Execute()
		{
			if (_stateMachine.CurrentState != _from)
				return;

			_stateMachine.SetState(_to);
		}

		readonly State _from;
		readonly StateMachine _stateMachine;
		readonly State _to;
	}


	public class App : MonoBehaviour
	{
		[Header("Dependencies")]
		[field: SerializeReference] public UI UI { get; private set; }

		[field: SerializeReference] public User User { get; private set; }
		[field: SerializeReference] public DominantFoot DominantFoot { get; private set; }

		[Header("Prefabs")]
		public Opponent OpponentPrefab;

		public Ball BallPrefab;

		[Header("Settings")]
		[field: SerializeReference] public bool RecordVideo { get; private set; }

		[Header("Internal state")]
		public IWebcamRecorder WebcamRecorder { get; private set; }

		public WebCamConfiguration WebCamConfiguration { get; set; }
		public Experiment Experiment { get; private set; }
		public Trial CurrentTrial { get; set; }
		public Opponent Opponent { get; set; }
		public Ball Ball { get; set; }


		public Transitions Transitions { get; private set; }

		public StateMachine StateMachine { get; set; }

		void Start()
		{


			// Get services for app
			WebcamRecorder = ServiceLocator.Get<IWebcamRecorder>();

			Experiment = new Experiment();
			StateMachine = new StateMachine();
			Transitions = new Transitions();

			// Setup app behaviour
			var init = new InitState(this);
			var startRecording = new StartRecordingVideoState(this);
			var trial = new TrialState(this);
			var end = new TrialEndState(this);
			
			Transitions.StartRecording = new Transition(this, init, startRecording);
			Transitions.StartTrial = new Transition(this, startRecording, trial);
			Transitions.EndTrial = new Transition(this, trial, end);


			// // Adding transitions based on predicates
			// init.AddTransition(new Transition(startRecording, new CompositePredicate(RecordVideo, ref Events.NextTrialRequested)));
			// init.AddTransition(new Transition(trial, new CompositePredicate(!RecordVideo, ref Events.NextTrialRequested)));
			// startRecording.AddTransition(new Transition(trial, new EventPredicate(ref Events.RecordingStarted)));
			// trial.AddTransition(new Transition(end, new EventPredicate(ref Events.TrialEnded)));
			// end.AddTransition(new Transition(init, new EventPredicate(ref Events.NextTrialRequested)));

			// Start app
			StateMachine.SetState(init);
		}

		void Update()
		{
			StateMachine.Tick();
		}
	}

	// third, create command based state changes!
	// second, create selector state with a list of available webcams

}
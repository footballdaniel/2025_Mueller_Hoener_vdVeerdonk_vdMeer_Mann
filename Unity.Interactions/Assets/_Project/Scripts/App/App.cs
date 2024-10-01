using App.States;
using Domain;
using Domain.VideoRecorder;
using UI;
using UnityEngine;

namespace App
{
	public class App : MonoBehaviour
	{
		[Header("Settings")]
		[field: SerializeReference] public bool RecordVideo { get; private set; }

		[Header("MonoBehaviours")]
		public MainUI UI { get; private set; }

		public User User { get; private set; }
		public DominantFoot DominantFoot { get; private set; }

		[Header("Prefabs")]
		public Opponent OpponentPrefab { get; private set; }

		public Ball BallPrefab { get; private set; }

		[Header("Internal state")]
		public IWebcamRecorder WebcamRecorder { get; private set; }

		public WebCamConfiguration WebCamConfiguration { get; set; }
		public Experiment Experiment { get; private set; }
		public Trial CurrentTrial { get; set; }
		public Opponent Opponent { get; set; }
		public Ball Ball { get; set; }

		[Header("State Machine")]
		public Transitions Transitions { get; private set; }

		public StateMachine StateMachine { get; set; }

		void Start()
		{
			// Monobehaviors
			WebcamRecorder = ServiceLocator.Get<IWebcamRecorder>();
			UI = ServiceLocator.Get<MainUI>();
			User = ServiceLocator.Get<User>();
			DominantFoot = ServiceLocator.Get<DominantFoot>();
		
			// Prefabs
			OpponentPrefab = ServiceLocator.Get<Opponent>();
			BallPrefab = ServiceLocator.Get<Ball>();
		
			// State machine
			StateMachine = new StateMachine();
			Transitions = new Transitions();
			Experiment = new Experiment();
		
			// States
			var init = new InitState(this);
			var webcamSelection = new WebcamSelectionState(this);
			var startRecording = new StartRecordingVideoState(this);
			var trial = new TrialState(this);
			var end = new TrialEndState(this);
		
			// Transitions
			Transitions.StartTrial = new Transition(this, init, trial);
			Transitions.RecordVideo = new Transition(this, init, webcamSelection);
			Transitions.StartRecording = new Transition(this, webcamSelection, startRecording);
			Transitions.StartTrial = new Transition(this, startRecording, trial);
			Transitions.EndTrial = new Transition(this, trial, end);
		
			// Start app
			StateMachine.SetState(init);
		}
		
		void Update()
		{
			StateMachine.Tick();
		}
	}

}
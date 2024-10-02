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
		public IWebcamRecorder WebcamRecorder { get; set; }
		public Experiment Experiment { get; private set; }
		public Trial CurrentTrial { get; set; }
		public Opponent Opponent { get; set; }
		public Ball Ball { get; set; }

		[Header("State Machine")]
		public Transitions Transitions { get; private set; }
		public StateMachine StateMachine { get; private set; }

		void Start()
		{
			// Monobehaviors
			UI = ServiceLocator.Get<MainUI>();
			User = ServiceLocator.Get<User>();
			DominantFoot = ServiceLocator.Get<DominantFoot>();
			WebCamRecorders = ServiceLocator.Get<IRepository<IWebcamRecorder>>();
		
			// Prefabs
			OpponentPrefab = ServiceLocator.Get<Opponent>();
			BallPrefab = ServiceLocator.Get<Ball>();
		
			// State machine
			StateMachine = new StateMachine();
			Transitions = new Transitions();
			Experiment = new Experiment();
		
			// States
			var init = new InitState(this);
			var webcamSelection = new SelectWebcam(this);
			var waitForNextTrial = new WaitForNextTrialState(this);
			var startRecording = new StartRecordingVideo(this);
			var trial = new TrialState(this);
			var end = new TrialEndState(this);
			var export = new ExportVideoState(this);
		
			// Flow for recording
			Transitions.SelectWebcam = new Transition(this, init, webcamSelection);
			Transitions.StartRecording = new Transition(this, webcamSelection, startRecording);
			Transitions.StartTrialWithRecording = new Transition(this, startRecording, trial);
			Transitions.ExportVideo = new Transition(this, trial, export);
			Transitions.FinishExport = new Transition(this, export, end);
			Transitions.WaitForNextTrial = new Transition(this, end, waitForNextTrial);
			
			// Flow without recording
			Transitions.BeginExperiment = new Transition(this, init, waitForNextTrial);
			Transitions.BeginNextTrial = new Transition(this, waitForNextTrial, trial);
			Transitions.EndTrial = new Transition(this, trial, end);
		
			// Start app
			StateMachine.SetState(init);
		}

		public IRepository<IWebcamRecorder> WebCamRecorders { get; set; }


		void Update()
		{
			StateMachine.Tick();
		}
	}
}
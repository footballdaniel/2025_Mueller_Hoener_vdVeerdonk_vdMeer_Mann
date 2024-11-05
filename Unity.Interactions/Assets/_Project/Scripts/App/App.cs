using App.States;
using DefaultNamespace;
using Domain;
using Domain.VideoRecorder;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace App
{
	public class App : MonoBehaviour
	{
		[Header("Settings")]
		public ExperimentalCondition ExperimentalCondition;

		public bool RecordVideo;
		public int RecordingFrameRateHz = 10;
		public Side DominantFootSide;

		[Header("Services")] public MainUI UI { get; private set; }
		public IRepository<IWebcamRecorder> WebCamRecorders { get; private set; }
		[Header("Entities")] public User User { get; private set; }
		public InSituOpponent InSituOpponent { get; private set; }
		[Header("Prefabs")] public Opponent OpponentPrefab { get; private set; }
		public Ball BallPrefab { get; private set; }

		[Header("State")]
		public Experiment Experiment { get; set; }

		public Transitions Transitions { get; private set; }
		public StateMachine StateMachine { get; private set; }

		void Start()
		{
			Experiment = new Experiment(RecordingFrameRateHz, DominantFootSide);

			// MonoBehaviours
			UI = ServiceLocator.Get<MainUI>();
			User = ServiceLocator.Get<User>();
			WebCamRecorders = ServiceLocator.Get<IRepository<IWebcamRecorder>>();
			InSituOpponent = ServiceLocator.Get<InSituOpponent>();

			// Prefabs
			OpponentPrefab = ServiceLocator.Get<Opponent>();
			BallPrefab = ServiceLocator.Get<Ball>();

			// State machine
			StateMachine = new StateMachine();
			Transitions = new Transitions();

			// States
			var xrStartupState = new XRStartupState(this);
			var init = new InitState(this);
			var webcamSelection = new SelectWebcamState(this);
			var waitForNextTrial = new WaitForNextTrialState(this);
			var initiateRecorder = new InitiateVideoRecorder(this);
			var export = new ExportVideoState(this);
			// laboratoryTrials
			var labTrial = new LabTrialState(this);
			var labTrialEnd = new LabTrialEndState(this);
			// in situ trials
			var inSituTrial = new InSituTrialState(this);
			
			// Flow for starting app
			Transitions.Init = new Transition(this, xrStartupState, init);

			// Flow for recording trials
			Transitions.SelectWebcam = new Transition(this, init, webcamSelection);
			Transitions.InitiateRecorder = new Transition(this, webcamSelection, initiateRecorder);
			Transitions.NextLabTrialWithVideoRecording = new Transition(this, initiateRecorder, waitForNextTrial);
			Transitions.NextInSituTrialWithVideoRecording = new Transition(this, initiateRecorder, waitForNextTrial);
			Transitions.ExportVideoOfLabTrial = new Transition(this, labTrial, export);
			Transitions.ExportVideoOfInSituTrial = new Transition(this, inSituTrial, export);
			Transitions.EndTrialLab = new Transition(this, export, labTrialEnd);
			Transitions.WaitForNextTrialInSitu = new Transition(this, export, initiateRecorder);
			Transitions.WaitForNextTrialLab = new Transition(this, labTrialEnd, initiateRecorder);

			// Flow without recording
			Transitions.BeginExperiment = new Transition(this, init, waitForNextTrial);
			Transitions.NextLabTrialWithoutRecording = new Transition(this, waitForNextTrial, labTrial);
			Transitions.NextInSituTrialWithoutRecording = new Transition(this, waitForNextTrial, inSituTrial);
			Transitions.EndLabTrial = new Transition(this, labTrial, labTrialEnd);
			Transitions.EndLabTrialAfterExporting = new Transition(this, export, labTrialEnd);
			Transitions.EndInSituTrial = new Transition(this, inSituTrial, waitForNextTrial);

			// Start app
			StateMachine.SetState(xrStartupState);
		}
		
		void Update()
		{
			StateMachine.Tick();
			
			// Cheats to transition between states
			if (Keyboard.current.digit1Key.wasPressedThisFrame)
				Transitions.Init.Execute();
		}
	}

}
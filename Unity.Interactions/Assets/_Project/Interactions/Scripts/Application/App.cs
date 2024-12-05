using Interactions.Application.States;
using Interactions.Application.Transitions;
using Interactions.Application.ViewModels;
using Interactions.Domain;
using Interactions.Domain.VideoRecorder;
using Interactions.UI;
using PassDetection.Replay;
using Tactive.MachineLearning.Models;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactions.Application
{
	public class App : MonoBehaviour
	{
		[Header("Settings")]
		public bool RecordVideo;
		public int RecordingFrameRateHz = 10;
		public Side DominantFootSide;
		public ModelAssetWithMetadata LstmModelAsset;
		public AudioClip PassSoundClip;

		[Header("Services")] public MainUI UI { get; private set; }
		public ExperimentalCondition ExperimentalCondition { get; set; }
		public IRepository<IWebcamRecorder> WebCamRecorders { get; private set; }
		public LstmModel LstmModel { get; private set; }
		[Header("Entities")] public User User { get; private set; }
		public InSituOpponent InSituOpponentPrefab { get; private set; }
		[Header("Prefabs")] public Opponent OpponentPrefab { get; private set; }
		public Ball BallPrefab { get; private set; }
		[Header("State")] public Experiment Experiment { get; set; }
		public Transitions.Transitions Transitions { get; private set; }
		public StateMachine StateMachine { get; private set; }

		public WebcamSelectionViewModel WebcamSelectionViewModel { get; private set; }
		

		void Start()
		{
			// Services
			Experiment = new Experiment(RecordingFrameRateHz, DominantFootSide);
			LstmModel = new LstmModel(LstmModelAsset);

			// MonoBehaviours
			UI = ServiceLocator.Get<MainUI>();
			User = ServiceLocator.Get<User>();
			WebCamRecorders = ServiceLocator.Get<IRepository<IWebcamRecorder>>();
			InSituOpponentPrefab = ServiceLocator.Get<InSituOpponent>();

			// Prefabs
			OpponentPrefab = ServiceLocator.Get<Opponent>();
			InSituOpponentPrefab = ServiceLocator.Get<InSituOpponent>();
			BallPrefab = ServiceLocator.Get<Ball>();

			// State machine
			StateMachine = new StateMachine();
			Transitions = new Transitions.Transitions();

			// States
			var xrStartupState = new XRStartup(this);
			var selectCondition = new ConditionSelection(this);
			var init = new InitState(this);
			var webcamSelection = new SelectWebcam(this);
			var waitForNextTrial = new WaitForNextTrial(this);
			var initiateRecorder = new InitiateVideoRecorder(this);
			var export = new ExportVideo(this);
			// laboratoryTrials
			var labTrial = new LabTrial(this);
			var labTrialEnd = new LabTrialEnd(this);
			// in situ trials
			var inSituTrial = new InSituTrial(this);

			// View models for showing data on the UI
			WebcamSelectionViewModel = new WebcamSelectionViewModel(this);

			// Flow for starting app
			Transitions.SelectCondition = new Transition(this, xrStartupState, selectCondition);
			Transitions.Init = new Transition(this, selectCondition, init);

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

			Cheats();
		}

		void Cheats()
		{
			if (Keyboard.current.digit1Key.wasPressedThisFrame)
				Transitions.SelectCondition.Execute();

			if (Keyboard.current.digit2Key.wasPressedThisFrame)
			{
				ExperimentalCondition = ExperimentalCondition.Laboratory;
				Transitions.Init.Execute();
			}

			if (Keyboard.current.digit3Key.wasPressedThisFrame)
			{
				var recorder = WebCamRecorders.Get(0);
				WebcamSelectionViewModel.Select(recorder);
				Transitions.InitiateRecorder.Execute();
			}
		}
	}

}
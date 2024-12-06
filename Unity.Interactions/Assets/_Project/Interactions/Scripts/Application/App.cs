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
		public int RecordingFrameRateHz = 10;

		public ModelAssetWithMetadata LstmModelAsset;
		public AudioClip PassSoundClip;

		[Header("Services")] public MainUI UI { get; private set; }
		public Side DominantFootSide { get; set; }
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
		public XRStatusViewModel XRStatusViewModel { get; private set; }
		public RightGoal RightGoal { get; private set; }
		public LeftGoal LeftGoal { get; private set; }
		public ExperimentViewModel ExperimentViewModel { get; private set; }

		void Start()
		{
			// Dependencies
			Experiment = new Experiment(RecordingFrameRateHz, DominantFootSide);
			LstmModel = new LstmModel(LstmModelAsset);

			// MonoBehaviours
			UI = ServiceLocator.Get<MainUI>();
			User = ServiceLocator.Get<User>();
			WebCamRecorders = ServiceLocator.Get<IRepository<IWebcamRecorder>>();
			InSituOpponentPrefab = ServiceLocator.Get<InSituOpponent>();
			LeftGoal = ServiceLocator.Get<LeftGoal>();
			RightGoal = ServiceLocator.Get<RightGoal>();

			// Prefabs
			OpponentPrefab = ServiceLocator.Get<Opponent>();
			InSituOpponentPrefab = ServiceLocator.Get<InSituOpponent>();
			BallPrefab = ServiceLocator.Get<Ball>();

			// View models for showing data on the UI
			WebcamSelectionViewModel = new WebcamSelectionViewModel(this);
			XRStatusViewModel = new XRStatusViewModel(this);
			ExperimentViewModel = new ExperimentViewModel(this);

			// State machine
			StateMachine = new StateMachine();
			Transitions = new Transitions.Transitions();

			// States
			var startupXr = new StartupXr(this);
			var startExperiment = new StartExperiment(this);
			var selectWebcam = new SelectWebcam(this);
			var waitForNextTrial = new WaitForNextTrial(this);
			var initiateRecorder = new InitiateVideoRecorder(this);
			var export = new ExportVideo(this);
			var labTrial = new LabTrial(this);
			var inSituTrial = new InSituTrial(this);

			// Flow for starting app
			Transitions.StartExperiment = new Transition(this, startupXr, startExperiment);
			Transitions.SelectWebcam = new Transition(this, startExperiment, selectWebcam);
			Transitions.InitiateRecorder = new Transition(this, selectWebcam, initiateRecorder);
			Transitions.WaitForNextTrial = new Transition(this, initiateRecorder, waitForNextTrial);
			Transitions.LaboratoryTrial = new Transition(this, waitForNextTrial, labTrial);
			Transitions.InSituTrial = new Transition(this, waitForNextTrial, inSituTrial);
			Transitions.ExportVideo = new Transition(this, labTrial, export);
			Transitions.ExportLA
			Transitions.WaitForNextTrial = new Transition(this, export, initiateRecorder);
			
			// Start app
			StateMachine.SetState(startupXr);
		}

		void Update()
		{
			StateMachine.Tick();
			Cheats();
		}

		void Cheats()
		{
			if (Keyboard.current.digit1Key.wasPressedThisFrame)
				Transitions.StartExperiment.Execute();


			if (Keyboard.current.digit2Key.wasPressedThisFrame)
			{
				ExperimentalCondition = ExperimentalCondition.Laboratory;
				var recorder = WebCamRecorders.Get(0);
				WebcamSelectionViewModel.Select(recorder);
				Transitions.InitiateRecorder.Execute();
			}
		}
	}

}
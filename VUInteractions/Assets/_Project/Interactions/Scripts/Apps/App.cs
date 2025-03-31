using Interactions.Apps.States;
using Interactions.Apps.StateTransitions;
using Interactions.Apps.ViewModels;
using Interactions.Domain;
using Interactions.Domain.DecisionMaking.Constraints;
using Interactions.Domain.Goals;
using Interactions.Domain.Opponents;
using Interactions.Domain.VideoRecorders;
using Interactions.Infra;
using Interactions.UI;
using PassDetection.Replay;
using Tactive.MachineLearning.Models;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactions.Apps
{
	public class App : MonoBehaviour
	{
		[Header("Settings")]
		public Experiment Experiment;
		public AudioClip PassSoundClip;
		[Header("Entities and Services")] public MainUI UI { get; private set; }
		public IRepository<IWebcamRecorder> WebCamRecorders { get; private set; }
		public LstmModel LstmModel { get; private set; }
		public User User { get; private set; }
		public LabEnvironment LabEnvironment { get; private set; }
		[Header("Prefabs")] public Opponent OpponentPrefab { get; private set; }
		public InSituOpponent InSituOpponentPrefab { get; private set; }
		public Ball BallPrefab { get; private set; }

		[Header("State")]
		public Side DominantFootSide { get; set; }
		public Transitions Transitions { get; private set; }
		public StateMachine StateMachine { get; private set; }
		public WebcamSelectionViewModel WebcamSelectionViewModel { get; private set; }
		public XRStatusViewModel XRStatusViewModel { get; private set; }
		public RightGoal RightGoal { get; private set; }
		public LeftGoal LeftGoal { get; private set; }
		public ExperimentViewModel ExperimentViewModel { get; private set; }
		public XRTrackers Trackers { get; set; }
		public OpponentSettingsViewModel OpponentSettingsViewModel { get; private set; }
		public IPassCorrector PassCorrector { get; set; }
		public OpponentMaximalPositionConstraint OpponentMaximalPositionConstraint { get; set; }
		public PassDetector PassDetector { get; set; }
		public Ball Ball { get; set; }

		void Start()
		{
			// MonoBehaviours
			UI = ServiceLocator.Get<MainUI>();
			User = ServiceLocator.Get<User>();
			WebCamRecorders = ServiceLocator.Get<IRepository<IWebcamRecorder>>();
			InSituOpponentPrefab = ServiceLocator.Get<InSituOpponent>();
			LeftGoal = ServiceLocator.Get<LeftGoal>();
			RightGoal = ServiceLocator.Get<RightGoal>();
			Trackers = ServiceLocator.Get<XRTrackers>();
			LabEnvironment = ServiceLocator.Get<LabEnvironment>();

			// Prefabs
			OpponentPrefab = ServiceLocator.Get<Opponent>();
			InSituOpponentPrefab = ServiceLocator.Get<InSituOpponent>();
			BallPrefab = ServiceLocator.Get<Ball>();

			// Other Dependencies
			var lstmModelAsset = ServiceLocator.Get<ModelAssetWithMetadata>();
			Experiment.Bind(DominantFootSide, LeftGoal, RightGoal);
			LstmModel = new LstmModel(lstmModelAsset);
			OpponentMaximalPositionConstraint = new OpponentMaximalPositionConstraint(2);
			PassCorrector = new PassCorrector(User, Experiment.RightGoal, Experiment.LeftGoal);
			PassDetector = new PassDetector(this);

			// View models for showing data on the UI
			WebcamSelectionViewModel = new WebcamSelectionViewModel(this);
			XRStatusViewModel = new XRStatusViewModel(this);
			ExperimentViewModel = new ExperimentViewModel(this);
			OpponentSettingsViewModel = new OpponentSettingsViewModel(this);

			// State machine
			StateMachine = new StateMachine();
			Transitions = new Transitions();

			// States
			var startupXr = new StartupXr(this);
			var startExperiment = new StartExperiment(this);
			var selectWebcam = new SelectWebcam(this);
			var waitForNextTrial = new WaitForNextTrial(this);
			var labTrialInteractive = new LaboratoryTrialInteractive(this);
			var labTrialNonInteractive = new LaboratoryTrialNonInteractive(this);
			var labTrialNoOpponent = new LaboratoryTrialNoOpponent(this);
			var inSituTrial = new InSituTrial(this);

			// Flow for starting app
			Transitions.StartExperiment = new Transition(this, startupXr, startExperiment);
			Transitions.SelectWebcam = new Transition(this, startExperiment, selectWebcam);
			Transitions.WaitForNextTrial = new Transition(this, new State[] { selectWebcam, labTrialInteractive, labTrialNonInteractive, labTrialNoOpponent, inSituTrial }, waitForNextTrial);
			Transitions.LaboratoryTrialInteractive = new Transition(this, waitForNextTrial, labTrialInteractive);
			Transitions.LaboratoryTrialNonInteractive = new Transition(this, waitForNextTrial, labTrialNonInteractive);
			Transitions.LaboratoryNoOpponent = new Transition(this, waitForNextTrial, labTrialNoOpponent);
			Transitions.InSituTrial = new Transition(this, waitForNextTrial, inSituTrial);
			Transitions.Quit = new ImmediateTransition(this);

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
				Transitions.SelectWebcam.Execute();

			if (Keyboard.current.digit3Key.wasPressedThisFrame)
			{
				var recorder = WebCamRecorders.Get(0);
				WebcamSelectionViewModel.Select(recorder);
				Transitions.LaboratoryTrialInteractive.Execute();
			}

			if (Keyboard.current.digit4Key.wasPressedThisFrame)
				Transitions.LaboratoryTrialInteractive.Execute();

			if (Keyboard.current.digit5Key.wasPressedThisFrame)
				Transitions.InSituTrial.Execute();
		}
	}

}
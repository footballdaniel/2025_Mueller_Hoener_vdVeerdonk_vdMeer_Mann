using System.Collections.Generic;
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
		public Dictionary<ExperimentalCondition, Transition> TrialTransitions { get; private set; }
		Dictionary<ExperimentalCondition, IOpponentBehavior> _opponentBehaviors;
		public StateMachine StateMachine { get; private set; }
		public WebcamSelectionViewModel WebcamSelectionViewModel { get; private set; }
		public XRStatusViewModel XRStatusViewModel { get; private set; }
		public RightGoal RightGoal { get; private set; }
		public LeftGoal LeftGoal { get; private set; }
		public ExperimentViewModel ExperimentViewModel { get; private set; }
		public XRTrackers Trackers { get; set; }
		public OpponentSettingsViewModel OpponentSettingsViewModel { get; private set; }
		public ProactiveSettingsViewModel ProactiveSettingsViewModel { get; private set; }
		public IPassCorrector PassCorrector { get; set; }
		public OpponentMaximalPositionConstraint OpponentMaximalPositionConstraint { get; set; }
		public PassDetector PassDetector { get; set; }
		public Ball Ball { get; set; }
		public ExperimentConfig Config { get; private set; }

		void Start()
		{
			UI = ServiceLocator.Get<MainUI>();
			User = ServiceLocator.Get<User>();
			WebCamRecorders = ServiceLocator.Get<IRepository<IWebcamRecorder>>();
			InSituOpponentPrefab = ServiceLocator.Get<InSituOpponent>();
			LeftGoal = ServiceLocator.Get<LeftGoal>();
			RightGoal = ServiceLocator.Get<RightGoal>();
			Trackers = ServiceLocator.Get<XRTrackers>();
			LabEnvironment = ServiceLocator.Get<LabEnvironment>();

			OpponentPrefab = ServiceLocator.Get<Opponent>();
			InSituOpponentPrefab = ServiceLocator.Get<InSituOpponent>();
			BallPrefab = ServiceLocator.Get<Ball>();

			var lstmModelAsset = ServiceLocator.Get<ModelAssetWithMetadata>();
			Experiment.Bind(DominantFootSide, LeftGoal, RightGoal);
			ApplyExperimentConfig();
			LstmModel = new LstmModel(lstmModelAsset);
			OpponentMaximalPositionConstraint = new OpponentMaximalPositionConstraint(2);
			PassCorrector = new PassCorrector(User, Experiment.RightGoal, Experiment.LeftGoal);
			PassDetector = new PassDetector(this);

			WebcamSelectionViewModel = new WebcamSelectionViewModel(this);
			XRStatusViewModel = new XRStatusViewModel(this);
			ExperimentViewModel = new ExperimentViewModel(this);
			OpponentSettingsViewModel = new OpponentSettingsViewModel(this);
			ProactiveSettingsViewModel = new ProactiveSettingsViewModel(this);

			StateMachine = new StateMachine();
			Transitions = new Transitions();

			var startupXr = new StartupXr(this);
			var startExperiment = new StartExperiment(this);
			var selectWebcam = new SelectWebcam(this);
			var waitForNextTrial = new WaitForNextTrial(this);

			_opponentBehaviors = new Dictionary<ExperimentalCondition, IOpponentBehavior>
			{
				[ExperimentalCondition.LaboratoryInteractive] = new InteractiveBehavior(),
				[ExperimentalCondition.LaboratoryNonInteractive] = new NonInteractiveBehavior(),
				[ExperimentalCondition.LaboratoryProactiveInteractive] = new ProactiveBehavior(),
			};

			var trialStates = new Dictionary<ExperimentalCondition, State>
			{
				[ExperimentalCondition.LaboratoryInteractive] = new LaboratoryTrial(this, _opponentBehaviors[ExperimentalCondition.LaboratoryInteractive]),
				[ExperimentalCondition.LaboratoryNonInteractive] = new LaboratoryTrial(this, _opponentBehaviors[ExperimentalCondition.LaboratoryNonInteractive]),
				[ExperimentalCondition.LaboratoryProactiveInteractive] = new LaboratoryTrial(this, _opponentBehaviors[ExperimentalCondition.LaboratoryProactiveInteractive]),
				[ExperimentalCondition.LaboratoryNoOpponent] = new LaboratoryTrialNoOpponent(this),
				[ExperimentalCondition.InSitu] = new InSituTrial(this),
			};

			Transitions.StartExperiment = new Transition(this, startupXr, startExperiment);
			Transitions.SelectWebcam = new Transition(this, startExperiment, selectWebcam);

			var waitForNextTrialSources = new List<State> { selectWebcam };
			waitForNextTrialSources.AddRange(trialStates.Values);
			Transitions.WaitForNextTrial = new Transition(this, waitForNextTrialSources.ToArray(), waitForNextTrial);

			TrialTransitions = new Dictionary<ExperimentalCondition, Transition>();
			foreach (var entry in trialStates)
				TrialTransitions[entry.Key] = new Transition(this, waitForNextTrial, entry.Value);

			Transitions.Quit = new ImmediateTransition(this);

			StateMachine.SetState(startupXr);
		}


		void ApplyExperimentConfig()
		{
			Config = ExperimentConfig.Load();

			Experiment.InterPersonalDistance = Config.InterPersonalDistance;
			Experiment.BodyInformationWeight = Config.BodyInformationWeight;
			Experiment.FootInformationWeight = Config.FootInformationWeight;
			Experiment.OpponentAcceleration = Config.OpponentAcceleration;
			Experiment.OpponentReactionTimeBody = Config.OpponentReactionTimeBody;
			Experiment.OpponentReactionTimeFoot = Config.OpponentReactionTimeFoot;
			Experiment.DistanceBetweenGoals = Config.DistanceBetweenGoals;
			Experiment.PassDetectionThreshold = Config.PassDetectionThreshold;
		}

		public void StartTrialFor(ExperimentalCondition condition)
		{
			if (TrialTransitions.TryGetValue(condition, out var transition))
				transition.Execute();
			else
				Debug.Log($"No trial registered for condition {condition}");
		}

		public void ShowSettingsFor(ExperimentalCondition condition)
		{
			if (_opponentBehaviors.TryGetValue(condition, out var behavior))
			{
				UI.OpponentSettingsUI.Bind(behavior.GetSettings(this));
				UI.OpponentSettingsUI.Show();
			}
			else
			{
				UI.OpponentSettingsUI.Hide();
			}
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
				StartTrialFor(ExperimentalCondition.LaboratoryInteractive);
			}

			if (Keyboard.current.digit4Key.wasPressedThisFrame)
				StartTrialFor(ExperimentalCondition.LaboratoryInteractive);

			if (Keyboard.current.digit5Key.wasPressedThisFrame)
				StartTrialFor(ExperimentalCondition.InSitu);
		}
	}

}
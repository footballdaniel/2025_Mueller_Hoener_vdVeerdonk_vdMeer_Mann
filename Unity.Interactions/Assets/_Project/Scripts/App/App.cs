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
		public ExperimentalCondition ExperimentalCondition;
		public bool RecordVideo;

		[Header("Services")] public MainUI UI { get; private set; }
		public IRepository<IWebcamRecorder> WebCamRecorders { get; private set; }
		public IRepository<Teammate> Teammates { get; set; }
		[Header("Entities")] public User User { get; private set; }
		public DominantFoot DominantFoot { get; private set; }
		[Header("Prefabs")] public Opponent OpponentPrefab { get; private set; }
		public Ball BallPrefab { get; private set; }

		[Header("State")]
		public Transitions Transitions { get; private set; }
		public StateMachine StateMachine { get; private set; }
		public SessionState Session { get; private set; }
		

		void Start()
		{
			Session = new SessionState();

			// MonoBehaviours
			UI = ServiceLocator.Get<MainUI>();
			User = ServiceLocator.Get<User>();
			DominantFoot = ServiceLocator.Get<DominantFoot>();
			WebCamRecorders = ServiceLocator.Get<IRepository<IWebcamRecorder>>();
			Teammates = ServiceLocator.Get<IRepository<Teammate>>();

			// Prefabs
			OpponentPrefab = ServiceLocator.Get<Opponent>();
			BallPrefab = ServiceLocator.Get<Ball>();

			// State machine
			StateMachine = new StateMachine();
			Transitions = new Transitions();

			// States
			var init = new InitState(this);
			var webcamSelection = new SelectWebcamState(this);
			var waitForNextTrial = new WaitForNextTrialState(this);
			var initiateRecorder = new InitiateVideoRecorder(this);
			var export = new ExportVideoState(this);
			// laboratoryTrials
			var labTrial = new LabTrialState(this);
			var labTrialEnd = new TrialEndState(this);
			// in situ trials
			var inSituTrial = new InSituTrialState(this);

			// Flow for recording trials
			Transitions.SelectWebcam = new Transition(this, init, webcamSelection);
			Transitions.InitiateRecorder = new Transition(this, webcamSelection, initiateRecorder);
			Transitions.NextLabTrialWithVideoRecording = new Transition(this, initiateRecorder, labTrial);
			Transitions.NextInSituTrialWithVideoRecording = new Transition(this, initiateRecorder, inSituTrial);
			Transitions.ExportVideoOfLabTrial = new Transition(this, labTrial, export);
			Transitions.ExportVideoOfInSituTrial = new Transition(this, inSituTrial, export);
			Transitions.FinishExport = new Transition(this, export, labTrialEnd);
			Transitions.WaitForNextTrial = new Transition(this, labTrialEnd, waitForNextTrial);

			// Flow without recording
			Transitions.BeginExperiment = new Transition(this, init, waitForNextTrial);
			Transitions.NextLabTrialWithoutRecording = new Transition(this, waitForNextTrial, labTrial);
			Transitions.NextInSituTrialWithoutRecording = new Transition(this, waitForNextTrial, inSituTrial);
			
			Transitions.EndLabTrial = new Transition(this, labTrial, labTrialEnd);
			Transitions.EndInSituTrial = new Transition(this, inSituTrial, waitForNextTrial);

			// Start app
			StateMachine.SetState(init);
		}

		void Update()
		{
			StateMachine.Tick();
		}
	}
}
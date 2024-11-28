using Interactions.Scripts.Application.States;
using Interactions.Scripts.Application.ViewModels;
using UnityEngine;

namespace Interactions.Scripts.Application
{
	internal class InSituTrialState : State
	{
		float _updateTimer;

		public InSituTrialState(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_app.Experiment.NextTrial();

			var viewModel = new InSituTrialViewModel(_app);
			_app.UI.InSituUI.Bind(viewModel);
		}

		public override void Exit()
		{
			_app.Experiment.WebcamRecorder.StopRecording();
			_app.Experiment.CurrentTrial.Save();
		}

		public override void Tick()
		{
			var deltaTime = 1f / _app.RecordingFrameRateHz;
			_updateTimer += Time.fixedDeltaTime;
			var epsilon = 0.0001f;

			if (_updateTimer >= deltaTime - epsilon)
			{
				_app.Experiment.CurrentTrial.OpponentHipPositions.Add(_app.InSituOpponent.Hips);
				_app.Experiment.CurrentTrial.UserHeadPositions.Add(_app.User.Head.transform.position);
				_app.Experiment.CurrentTrial.UserDominantFootPositions.Add(_app.User.DominantFoot.transform.position);
				_app.Experiment.CurrentTrial.UserNonDominantFootPositions.Add(_app.User.NonDominantFoot.transform.position);

				_app.Experiment.WebcamRecorder.Tick();
				_app.Experiment.CurrentTrial.Tick(deltaTime);

				_updateTimer -= deltaTime;
			}
		}
	}
}
using Interactions.Application.States;
using Interactions.Application.ViewModels;
using Interactions.Domain;
using UnityEngine;

namespace Interactions.Application
{
	internal class InSituTrial : State
	{
		float _updateTimer;
		InSituOpponent _opponent;

		public InSituTrial(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_app.Experiment.NextTrial();
			_opponent = Object.Instantiate(_app.InSituOpponentPrefab);

			var viewModel = new InSituTrialViewModel(_app);
			_app.UI.InSituUI.Bind(viewModel);
			_app.UI.InSituUI.Show();
		}

		public override void Exit()
		{
			Object.Destroy(_opponent.gameObject);
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
				_app.Experiment.CurrentTrial.OpponentHipPositions.Add(_opponent.Hips);
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
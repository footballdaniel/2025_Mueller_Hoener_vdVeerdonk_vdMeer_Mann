using _Project.Interactions.Scripts.Domain;
using Interactions.Scripts.Infra;
using UnityEngine;

namespace Interactions.Scripts.Application.States
{
	internal class LabTrial : State
	{
		bool _hasPassed;
		InputDataQueue _inputDataQueue;
		float _updateTimer;

		public LabTrial(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_inputDataQueue = new InputDataQueue();
			_app.Experiment.NextTrial();
			_app.Experiment.Opponent = Object.Instantiate(_app.OpponentPrefab);
			_app.Experiment.Ball = Object.Instantiate(_app.BallPrefab);
			_app.Experiment.Opponent.Set(_app.User);

			_app.User.DominantFoot.Passed += OnPassed;
		}

		public override void Exit()
		{
			_hasPassed = false;
			_app.User.DominantFoot.Passed -= OnPassed;
		}

		public override void Tick()
		{
			var frameRateHz = 10f;
			var deltaTime = 1f / frameRateHz;
			_updateTimer += Time.fixedDeltaTime;
			var epsilon = 0.0001f;

			if (_updateTimer >= deltaTime - epsilon)
			{
				_app.Experiment.CurrentTrial.OpponentHipPositions.Add(_app.Experiment.Opponent.transform.position);
				_app.Experiment.CurrentTrial.UserHeadPositions.Add(_app.User.Head.transform.position);
				_app.Experiment.CurrentTrial.UserDominantFootPositions.Add(_app.User.DominantFoot.transform.position);
				_app.Experiment.CurrentTrial.UserNonDominantFootPositions.Add(_app.User.NonDominantFoot.transform.position);
				_app.Experiment.CurrentTrial.Tick(Time.deltaTime);

				_inputDataQueue.EnQueue(_app.User.DominantFoot.transform.position, _app.User.NonDominantFoot.transform.position, _app.Experiment.CurrentTrial.Duration);
				Debug.Log(_app.LstmModel.Evaluate(_inputDataQueue.ToInputData()));

				_app.Experiment.WebcamRecorder.Tick();
				_updateTimer -= deltaTime;
			}
		}

		void OnPassed(Pass pass)
		{
			if (_hasPassed) return;

			_hasPassed = true;
			_app.Experiment.Ball = Object.Instantiate(_app.BallPrefab, pass.Position, Quaternion.identity);
			_app.Experiment.Ball.Set(pass);
		}
	}
}
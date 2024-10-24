using App.States;
using UnityEngine;

namespace App
{
	internal class InSituTrialState : State
	{
		public InSituTrialState(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_app.TrialState.CurrentTrial = _app.TrialState.Experiment.NextTrial();
			
			var viewModel = new InSituViewModel(_app.TrialState.WebcamRecorder);
			_app.UI.InSituUI.Bind(viewModel);
		}

		public override void Tick()
		{
			// Saving
			_app.TrialState.CurrentTrial.OpponentHipPositions.Add(_app.InSituOpponent.Hips);
			_app.TrialState.CurrentTrial.UserHeadPositions.Add(_app.User.Head.transform.position);
			_app.TrialState.CurrentTrial.UserDominantFootPositions.Add(_app.User.DominantFoot.transform.position);
			_app.TrialState.CurrentTrial.UserNonDominantFootPositions.Add(_app.User.NonDominantFoot.transform.position);
			
			_app.TrialState.CurrentTrial.Tick(Time.deltaTime);

			if (_app.TrialState.CurrentTrial.Duration < 5f)
				return;

			switch (_app.RecordVideo)
			{
				case false:
					_app.Transitions.EndInSituTrial.Execute();
					break;
				case true:
					_app.Transitions.ExportVideoOfInSituTrial.Execute();
					break;
			}
		}
		
		public override void Exit()
		{
			_app.TrialState.CurrentTrial.Save();
		}
	}
}
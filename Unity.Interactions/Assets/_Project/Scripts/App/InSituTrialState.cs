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
			
			var viewModel = new InSituTrialViewModel(_app);
			_app.UI.InSituUI.Bind(viewModel);
		}

		public override void Tick()
		{
			_app.TrialState.CurrentTrial.OpponentHipPositions.Add(_app.InSituOpponent.Hips);
			_app.TrialState.CurrentTrial.UserHeadPositions.Add(_app.User.Head.transform.position);
			_app.TrialState.CurrentTrial.UserDominantFootPositions.Add(_app.User.DominantFoot.transform.position);
			_app.TrialState.CurrentTrial.UserNonDominantFootPositions.Add(_app.User.NonDominantFoot.transform.position);
			
			_app.TrialState.CurrentTrial.Tick(Time.deltaTime);
		}
		
		public override void Exit()
		{
			_app.TrialState.CurrentTrial.Save();
		}
	}
}
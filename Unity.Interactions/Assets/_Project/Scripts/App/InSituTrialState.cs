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
			_app.Session.CurrentTrial = _app.Session.Experiment.NextTrial();

			var viewModel = new InSituViewModel();
			_app.UI.InSituUI.Bind(viewModel);
		}

		public override void Tick()
		{
			_app.Session.CurrentTrial.Tick(Time.deltaTime);

			if (_app.Session.CurrentTrial.Duration < 5f)
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
	}
}
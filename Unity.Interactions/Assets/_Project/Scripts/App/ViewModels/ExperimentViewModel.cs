using System;

namespace App
{
	public class ExperimentViewModel
	{
		public ExperimentViewModel(App app)
		{
			_app = app;
		}

		public ProgressIndicator Progress { get; } = ProgressIndicator.Instance;

		public Observable<bool> CanStartNextTrial { get; } = new(false);
		
		public void NextTrial()
		{
			CanStartNextTrial.Value = false;
			
			if (_app.ExperimentalCondition == ExperimentalCondition.Laboratory)
				_app.Transitions.NextLabTrialWithoutRecording.Execute();
			else
				_app.Transitions.NextInSituTrialWithoutRecording.Execute();
		}

		readonly App _app;

		public void StopTrial()
		{
			if (CanStartNextTrial.Value == true)
			{
				if (_app.ExperimentalCondition == ExperimentalCondition.Laboratory)
					_app.Transitions.EndLabTrial.Execute();
				if (_app.ExperimentalCondition == ExperimentalCondition.InSitu)
					_app.Transitions.EndInSituTrial.Execute();
			}
		}
	}
}
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

		public event Action<bool> TrialStatusChanged;
		
		public void NextTrial()
		{
			TrialStatusChanged?.Invoke(false);
			
			if (_app.ExperimentalCondition == ExperimentalCondition.Laboratory)
				_app.Transitions.NextLabTrialWithoutRecording.Execute();
			else
				_app.Transitions.NextInSituTrialWithoutRecording.Execute();
		}

		readonly App _app;
	}
}
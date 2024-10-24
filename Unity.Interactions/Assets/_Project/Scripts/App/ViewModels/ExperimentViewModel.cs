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
			_app.Transitions.BeginNextTrial.Execute();
		}

		readonly App _app;
	}
}
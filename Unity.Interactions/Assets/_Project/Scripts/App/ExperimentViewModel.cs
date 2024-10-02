namespace App
{
	public class ExperimentViewModel
	{
		public ExperimentViewModel(App app)
		{
			_app = app;
		}

		public ProgressIndicator Progress { get; } = ProgressIndicator.Instance;
		
		public Observable<bool> CanStartNextTrial { get; set; } = new(false);

		public void NextTrial()
		{
			_app.Transitions.BeginNextTrial.Execute();
		}

		readonly App _app;
	}
}
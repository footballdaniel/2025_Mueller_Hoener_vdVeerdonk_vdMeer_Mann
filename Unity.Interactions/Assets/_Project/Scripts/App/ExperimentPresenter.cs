using UnityEngine;

namespace App
{
	public class ExperimentPresenter
	{
		public ExperimentPresenter(App app)
		{
			_app = app;
		}

		public Observable<bool> CanStartNextTrial { get; set; } = new(false);

		public void NextTrial()
		{
			_app.Transitions.BeginNextTrial.Execute();
		}

		readonly App _app;
	}
}
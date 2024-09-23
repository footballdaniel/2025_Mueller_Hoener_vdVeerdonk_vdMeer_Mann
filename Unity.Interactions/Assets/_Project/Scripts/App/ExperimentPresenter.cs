using System.Collections.Generic;

namespace _Project.Scripts.App
{
	public class ExperimentPresenter
	{

		public ExperimentPresenter(AvailableWebCams availableWebCams, App app)
		{
			_app = app;
			
			AvailableWebCams = availableWebCams;
		}

		public List<WebCamConfiguration> AvailableWebCams { get; private set; }
		public Observable<bool> CanStartNextTrial { get; set; } = new(false);

		public void NextTrial()
		{
			_app.Transitions.StartTrial.Execute();
		}

		readonly App _app;

		public void Select(WebCamConfiguration webcam)
		{
			_app.WebCamConfiguration = webcam;
			_app.Transitions.StartRecording.Execute();
		}
	}
}
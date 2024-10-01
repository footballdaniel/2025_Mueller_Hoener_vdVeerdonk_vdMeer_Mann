using System.Collections.Generic;
using App.States;
using Domain.VideoRecorder;

namespace App
{
	internal class WebcamSelectionState : State
	{
		public WebcamSelectionState(App app) : base(app)
		{
		}

		public override void Enter()
		{
			var availableWebCams = new AvailableWebCams();
			var presenter = new WebcamSelectionPresenter(availableWebCams, _app);
			_app.UI.WebcamSelectionUI.Set(presenter);
		}
	}

	public class WebcamSelectionPresenter
	{
		public WebcamSelectionPresenter(AvailableWebCams availableWebCams, App app)
		{
			_app = app;

			AvailableWebCams = availableWebCams;
		}

		public List<WebCamConfiguration> AvailableWebCams { get; private set; }
		

		public void Select(WebCamConfiguration webcam)
		{
			_app.WebCamConfiguration = webcam;	
			_app.Transitions.StartTrial.Execute();
		}

		readonly App _app;

	}
}
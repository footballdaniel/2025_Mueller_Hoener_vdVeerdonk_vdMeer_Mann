using System.Collections.Generic;
using Domain.VideoRecorder;

namespace App
{
	public class WebcamSelectionPresenter
	{
		public WebcamSelectionPresenter(IRepository<WebCamConfiguration> webcamRepository, App app)
		{
			_app = app;

			WebcamRepository = webcamRepository.GetAll();
		}

		public IEnumerable<WebCamConfiguration> WebcamRepository { get; private set; }
		

		public void Select(WebCamConfiguration webcam)
		{
			_app.WebCamConfiguration = webcam;	
			_app.Transitions.StartRecording.Execute();
		}

		readonly App _app;

	}
}
using System.Collections.Generic;
using Domain.VideoRecorder;

namespace App
{
	public class WebcamSelectionPresenter
	{
		public WebcamSelectionPresenter(IRepository<IWebcamRecorder> webcamRepository, App app)
		{
			_app = app;

			Webcams = webcamRepository.GetAll();
		}

		public IEnumerable<IWebcamRecorder> Webcams { get; private set; }
		

		public void Select(IWebcamRecorder webcam)
		{
			_app.WebcamRecorder = webcam;	
			_app.Transitions.StartRecording.Execute();
		}

		readonly App _app;

	}
}
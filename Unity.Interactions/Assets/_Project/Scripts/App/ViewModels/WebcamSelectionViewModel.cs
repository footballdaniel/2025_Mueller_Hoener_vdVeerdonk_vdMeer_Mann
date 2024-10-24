using System.Collections.Generic;
using Domain.VideoRecorder;

namespace App
{
	public class WebcamSelectionViewModel
	{
		public WebcamSelectionViewModel(IRepository<IWebcamRecorder> webcamRepository, App app)
		{
			_app = app;
			Webcams = webcamRepository.GetAll();
		}

		public IEnumerable<IWebcamRecorder> Webcams { get; private set; }
		

		public void Select(IWebcamRecorder webcam)
		{
			_app.TrialState.WebcamRecorder = webcam;	
			_app.Transitions.InitiateRecorder.Execute();
		}

		readonly App _app;

	}
}
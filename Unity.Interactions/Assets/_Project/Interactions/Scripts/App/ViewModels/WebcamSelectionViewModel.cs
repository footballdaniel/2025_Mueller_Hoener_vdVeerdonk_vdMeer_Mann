using System.Collections.Generic;
using _Project.Interactions.Scripts.Domain.VideoRecorder;

namespace _Project.Interactions.Scripts.App.ViewModels
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
			_app.Experiment.WebcamRecorder = webcam;
			_app.Experiment.WebcamRecorder.RecordWith(_app.RecordingFrameRateHz);
			_app.Transitions.InitiateRecorder.Execute();
		}

		readonly App _app;
	}
}
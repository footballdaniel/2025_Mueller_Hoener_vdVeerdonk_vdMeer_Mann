using System.Collections.Generic;
using Interactions.Domain.VideoRecorder;

namespace Interactions.Application.ViewModels
{
	public class WebcamSelectionViewModel
	{
		public WebcamSelectionViewModel( App app)
		{
			_app = app;
			Webcams = app.WebCamRecorders.GetAll();
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
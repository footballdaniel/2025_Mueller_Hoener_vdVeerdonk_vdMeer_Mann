using System.Collections.Generic;
using Interactions.Domain.VideoRecorders;

namespace Interactions.Apps.ViewModels
{
	public class WebcamSelectionViewModel
	{
		public WebcamSelectionViewModel(App app)
		{
			_app = app;

			app.WebCamRecorders.Add(new NoWebcamRecorder());

			Webcams = app.WebCamRecorders.GetAll();
		}

		public IEnumerable<IWebcamRecorder> Webcams { get; private set; }

		public void Select(IWebcamRecorder webcam)
		{
			_app.Experiment.WebcamRecorder = webcam;
			_app.Experiment.WebcamRecorder.Initiate();
		}

		readonly App _app;
	}
}
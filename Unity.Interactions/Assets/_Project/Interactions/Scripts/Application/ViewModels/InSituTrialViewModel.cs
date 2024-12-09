using Interactions.Domain.VideoRecorder;
using UnityEngine;

namespace Interactions.Application.ViewModels
{
	public class InSituTrialViewModel
	{

		public InSituTrialViewModel(App app)
		{
			_app = app;
		}

		public Texture2D Frame => _app.Experiment.WebcamRecorder?.Frame ?? Texture2D.blackTexture;

		public void StopTrial()
		{
			_app.Transitions.ExportVideo.Execute();
		}

		readonly App _app;
		readonly IWebcamRecorder _recorder;
	}
}
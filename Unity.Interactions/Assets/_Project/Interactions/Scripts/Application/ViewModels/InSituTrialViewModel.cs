using _Project.Interactions.Scripts.Domain.VideoRecorder;
using UnityEngine;

namespace Interactions.Scripts.Application.ViewModels
{
	public class InSituTrialViewModel
	{
		readonly global::Interactions.Scripts.Application.App _app;
		readonly IWebcamRecorder _recorder;

		public InSituTrialViewModel(global::Interactions.Scripts.Application.App app)
		{
			_app = app;
		}

		public Texture2D Frame => _app.Experiment.WebcamRecorder?.Frame ?? Texture2D.blackTexture;

		public void StopTrial()
		{
			switch (_app.RecordVideo)
			{
				case false:
						_app.Transitions.EndInSituTrial.Execute();
					break;
				case true:
					_app.Transitions.ExportVideoOfInSituTrial.Execute();
					break;
			}
		}
	}
}
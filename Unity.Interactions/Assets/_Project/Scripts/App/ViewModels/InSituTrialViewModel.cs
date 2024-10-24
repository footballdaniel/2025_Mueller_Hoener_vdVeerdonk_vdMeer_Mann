using Domain.VideoRecorder;
using UnityEngine;

namespace App
{
	public class InSituTrialViewModel
	{
		readonly App _app;
		readonly IWebcamRecorder _recorder;

		public InSituTrialViewModel(App app)
		{
			_app = app;
		}

		public Texture2D Frame => _app.TrialState.WebcamRecorder?.Frame ?? Texture2D.blackTexture;

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
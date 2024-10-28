using UnityEngine;

namespace App
{
	public class ExperimentViewModel
	{
		readonly App _app;

		public ExperimentViewModel(App app)
		{
			_app = app;
		}

		public ProgressIndicator Progress { get; } = ProgressIndicator.Instance;
		public Observable<bool> CanStartNextTrial { get; } = new(true);

		public void NextTrial()
		{
			CanStartNextTrial.Value = false;

			if (_app.ExperimentalCondition == ExperimentalCondition.Laboratory)
				_app.Transitions.NextLabTrialWithoutRecording.Execute();
			else
				_app.Transitions.NextInSituTrialWithoutRecording.Execute();
		}

		public void StopTrial()
		{
			if (CanStartNextTrial.Value != false)
				return;

			switch (_app)
			{
				case { ExperimentalCondition: ExperimentalCondition.Laboratory, RecordVideo: true }:
					_app.Transitions.ExportVideoOfLabTrial.Execute();
					break;

				case { ExperimentalCondition: ExperimentalCondition.Laboratory, RecordVideo: false }:
					_app.Transitions.EndLabTrial.Execute();
					break;

				case { ExperimentalCondition: ExperimentalCondition.InSitu, RecordVideo: true }:
					_app.Transitions.ExportVideoOfInSituTrial.Execute();
					break;

				case { ExperimentalCondition: ExperimentalCondition.InSitu, RecordVideo: false }:
					_app.Transitions.EndInSituTrial.Execute();
					break;
			}
				
			CanStartNextTrial.Value = true;
		}

		public void ShowData()
		{
			var path = Application.persistentDataPath;
			var uri = new System.Uri(path);
			Application.OpenURL(uri.AbsoluteUri);
		}

		public void Exit()
		{
			if (Application.isEditor)
				UnityEditor.EditorApplication.isPlaying = false;
			else
				Application.Quit();
		}
	}
}
using System;
using Interactions.Domain.VideoRecorder;
using UnityEditor;

namespace Interactions.Application.ViewModels
{
	public class ExperimentViewModel
	{

		public ExperimentViewModel(App app)
		{
			_app = app;
		}

		public ProgressIndicator Progress { get; } = ProgressIndicator.Instance;
		public Observable<bool> CanStartNextTrial { get; } = new(true);

		public void Exit()
		{
#if UNITY_EDITOR
			if (UnityEngine.Application.isEditor)
				EditorApplication.isPlaying = false;
			else
#endif
				UnityEngine.Application.Quit();
		}

		public void NextTrial()
		{
			CanStartNextTrial.Value = false;
			
			if (_app.ExperimentalCondition == ExperimentalCondition.Laboratory)
				_app.Transitions.LaboratoryTrial.Execute();
			else
				_app.Transitions.InSituTrial.Execute();
		}

		public void ShowData()
		{
			var path = UnityEngine.Application.persistentDataPath;
			var uri = new Uri(path);
			UnityEngine.Application.OpenURL(uri.AbsoluteUri);
		}

		public void StopTrial()
		{
			if (CanStartNextTrial.Value != false)
				return;
			
			_app.Transitions.ExportVideo.Execute();

			CanStartNextTrial.Value = true;
		}

		readonly App _app;
	}
}
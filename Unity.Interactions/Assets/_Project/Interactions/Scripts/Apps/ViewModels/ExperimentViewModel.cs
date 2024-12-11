using System;
using Interactions.Domain.VideoRecorder;
using UnityEditor;
using UnityEngine;

namespace Interactions.Apps.ViewModels
{
	public class ExperimentViewModel
	{

		public ExperimentViewModel(App app)
		{
			_app = app;
		}

		public ProgressIndicator Progress { get; } = ProgressIndicator.Instance;
		public Observable<bool> CanStartNextTrial { get; } = new(true);
		public XRTracker HeadTracker => _app.Trackers.HeadTracker;
		public XRTracker DominantFootTracker => _app.Trackers.DominantFootTracker;
		public XRTracker NonDominantFootTracker => _app.Trackers.NonDominantFootTracker;
		public XRTracker DefenderHipsTracker => _app.Trackers.DefenderHipsTracker;

		public RenderTexture Frame => _app.Experiment.WebcamRecorder?.Frame ?? new RenderTexture(1, 1, 0, RenderTextureFormat.ARGB32);

		public void Exit()
		{
#if UNITY_EDITOR
			if (Application.isEditor)
				EditorApplication.isPlaying = false;
			else
#endif
				Application.Quit();
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
			var path = Application.persistentDataPath;
			var uri = new Uri(path);
			Application.OpenURL(uri.AbsoluteUri);
		}

		public void StopTrial()
		{
			if (CanStartNextTrial.Value != false)
				return;

			_app.Transitions.WaitForNextTrial.Execute();

			CanStartNextTrial.Value = true;
		}

		readonly App _app;
	}
}
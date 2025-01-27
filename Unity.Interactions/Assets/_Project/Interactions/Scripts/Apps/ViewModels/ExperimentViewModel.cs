using System;
using Interactions.Domain;
using UnityEngine;

namespace Interactions.Apps.ViewModels
{
	public class ExperimentViewModel
	{

		public ExperimentViewModel(App app)
		{
			_app = app;
		}

		public Observable<bool> CanStartNextTrial { get; } = new(true);
		public XRTracker HeadTracker => _app.Trackers.HeadTracker;
		public XRTracker DominantFootTracker => _app.Trackers.DominantFootTracker;
		public XRTracker NonDominantFootTracker => _app.Trackers.NonDominantFootTracker;
		public XRTracker DefenderHipsTracker => _app.Trackers.DefenderHipsTracker;
		public XRTracker UserHipsTracker => _app.Trackers.UserHipsTracker;
		public RenderTexture Frame => _app.Experiment.WebcamRecorder?.Frame ?? new RenderTexture(1, 1, 0, RenderTextureFormat.ARGB32);
		public int CurrentTrialIndex => _app.Experiment.CurrentTrialIndex;
		public bool IsLabEnvironmentVisible => _app.LabEnvironment.IsVisible;
		public float PassProbabilityDetectionThreshold => _app.Experiment.PassDetectionThreshold;


		public void NextTrial()
		{
			CanStartNextTrial.Value = false;

			switch (_app.Experiment.ExperimentalCondition)
			{
				case ExperimentalCondition.LaboratoryInteractive:
					_app.Transitions.LaboratoryTrialInteractive.Execute();
					break;
				case ExperimentalCondition.LaboratoryNonInteractive:
					_app.Transitions.LaboratoryTrialNonInteractive.Execute();
					break;
				case ExperimentalCondition.LaboratoryNoOpponent:
					_app.Transitions.LaboratoryNoOpponent.Execute();
					break;
				case ExperimentalCondition.InSitu:
					_app.Transitions.InSituTrial.Execute();
					break;
				default:
					Debug.Log("No trial type selected");
					break;
			}
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

		public void TogglePassCorrection(bool shouldCorrect)
		{
			if (shouldCorrect)
				_app.PassCorrector = new PassCorrector(_app.User, _app.Experiment.RightGoal, _app.Experiment.LeftGoal);
			else
				_app.PassCorrector = new NoPassCorrector();
		}

		public void ToggleLaboratoryEnvironmentVisibility(bool shouldShow)
		{
			if (shouldShow)
				_app.LabEnvironment.Show();
			else
				_app.LabEnvironment.Hide();
		}

		public void ContainsError()
		{
			_app.Experiment.CurrentTrial.ContainsError = true;
		}

		public void ChangePassProbabilityDetectionThreshold(float arg0)
		{
			_app.Experiment.PassDetectionThreshold = arg0;
		}
	}

}
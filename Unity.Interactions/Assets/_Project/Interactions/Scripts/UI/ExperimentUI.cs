using System;
using Interactions.Apps.ViewModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interactions.UI
{
	public class ExperimentUI : UIScreen
	{
		[SerializeField] RawImage _cameraFeed;
		[SerializeField] TMP_Text _fpsText;
		[SerializeField] TMP_Text _xrStatusText;
		[SerializeField] Button _nextTrialButton;
		[SerializeField] Button _stopTrialButton;
		[SerializeField] Button _showDataButton;
		[SerializeField] Button _exitButton;
		[SerializeField] XRTrackerStatus _xrTrackerStatusPrefab;
		[SerializeField] RectTransform _xrTrackerStatusContainer;
		[SerializeField] Toggle _passCorrectionToggle;

		void Update()
		{
			_fpsText.text = $"FPS: {Math.Round(1 / Time.deltaTime)} FPS Fixed: {Math.Round(1 / Time.fixedDeltaTime)}";
			_cameraFeed.texture = _viewmodel.Frame;
		}

		public void Bind(ExperimentViewModel viewModel)
		{
			Unbind();

			_viewmodel = viewModel;
			_cameraFeed.gameObject.SetActive(true);

			_nextTrialButton.onClick.AddListener(viewModel.NextTrial);
			_stopTrialButton.onClick.AddListener(viewModel.StopTrial);
			_showDataButton.onClick.AddListener(viewModel.ShowData);
			_exitButton.onClick.AddListener(viewModel.Exit);
			_passCorrectionToggle.onValueChanged.AddListener(viewModel.TogglePassCorrection);

			_stopTrialButton.interactable = false;

			viewModel.CanStartNextTrial.ValueChanged += OnCanStartNextTrialChanged;

			var headTracker = Instantiate(_xrTrackerStatusPrefab, _xrTrackerStatusContainer);
			var dominantFootTracker = Instantiate(_xrTrackerStatusPrefab, _xrTrackerStatusContainer);
			var nonDominantFootTracker = Instantiate(_xrTrackerStatusPrefab, _xrTrackerStatusContainer);
			var userHipsTracker = Instantiate(_xrTrackerStatusPrefab, _xrTrackerStatusContainer);
			var defenderHipsTracker = Instantiate(_xrTrackerStatusPrefab, _xrTrackerStatusContainer);

			dominantFootTracker.Bind(viewModel.DominantFootTracker);
			headTracker.Bind(viewModel.HeadTracker);
			nonDominantFootTracker.Bind(viewModel.NonDominantFootTracker);
			userHipsTracker.Bind(viewModel.UserHipsTracker);
			defenderHipsTracker.Bind(viewModel.DefenderHipsTracker);
		}

		void OnCanStartNextTrialChanged(bool canStart)
		{
			_stopTrialButton.interactable = !canStart;
			_nextTrialButton.interactable = canStart;
		}


		void Unbind()
		{
			_nextTrialButton.onClick.RemoveAllListeners();
			_stopTrialButton.onClick.RemoveAllListeners();
			_showDataButton.onClick.RemoveAllListeners();
			_exitButton.onClick.RemoveAllListeners();

			if (_viewmodel != null)
				_viewmodel.CanStartNextTrial.ValueChanged -= OnCanStartNextTrialChanged;

			foreach (Transform child in _xrTrackerStatusContainer)
				Destroy(child.gameObject);
		}

		bool _shouldUpdate;
		ExperimentViewModel _viewmodel;
	}
}
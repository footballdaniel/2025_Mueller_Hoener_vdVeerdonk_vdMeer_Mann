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
		[SerializeField] Button _containsError;
		[SerializeField] TMP_Text _trialNumberText;
		[SerializeField] Slider _passProbabilitySlider;
		[SerializeField] XRTrackerStatus _xrTrackerStatusPrefab;
		[SerializeField] RectTransform _xrTrackerStatusContainer;
		[SerializeField] Toggle _passCorrectionToggle;
		[SerializeField] Toggle _labEnvironmentVisibilityToggle;
		[SerializeField] Button _laboratoryInteractiveButton;
		[SerializeField] Button _laboratoryNonInteractiveButton;
		[SerializeField] Button _laboratoryNoOpponentButton;

		void Update()
		{
			_fpsText.text = $"FPS: {Math.Round(1 / Time.deltaTime)} FPS Fixed: {Math.Round(1 / Time.fixedDeltaTime)}";
			_cameraFeed.texture = _viewModel.Frame;
		}

		public void Bind(ExperimentViewModel viewModel)
		{
			Unbind();

			_viewModel = viewModel;
			_cameraFeed.gameObject.SetActive(true);
			_trialNumberText.SetText($"Trial {_viewModel.CurrentTrialIndex + 1}");

			_nextTrialButton.onClick.AddListener(viewModel.NextTrial);
			_stopTrialButton.onClick.AddListener(viewModel.StopTrial);
			_showDataButton.onClick.AddListener(viewModel.ShowData);
			_containsError.onClick.AddListener(OnContainsError);
			
			_passCorrectionToggle.onValueChanged.AddListener(viewModel.TogglePassCorrection);
			_labEnvironmentVisibilityToggle.onValueChanged.AddListener(viewModel.ToggleLaboratoryEnvironmentVisibility);
			_passProbabilitySlider.onValueChanged.AddListener(viewModel.ChangePassProbabilityDetectionThreshold);
			_passProbabilitySlider.SetValueWithoutNotify(viewModel.PassProbabilityDetectionThreshold);

			viewModel.TogglePassCorrection(_passCorrectionToggle.isOn);
			_stopTrialButton.interactable = false;

			viewModel.CanStartNextTrial.ValueChanged += OnCanStartNextTrialChanged;
			
			_laboratoryInteractiveButton.onClick.AddListener(viewModel.SelectInteractiveCondition);
			_laboratoryNonInteractiveButton.onClick.AddListener(viewModel.SelectNonInteractiveCondition);
			_laboratoryNoOpponentButton.onClick.AddListener(viewModel.SelectNoOpponentCondition);

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

		void OnContainsError()
		{
			_viewModel.ContainsError();
			_containsError.interactable = false;
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
			_passCorrectionToggle.onValueChanged.RemoveAllListeners();
			_labEnvironmentVisibilityToggle.onValueChanged.RemoveAllListeners();
			_containsError.onClick.RemoveAllListeners();
			_passProbabilitySlider.onValueChanged.RemoveAllListeners();
			
			_laboratoryInteractiveButton.onClick.RemoveAllListeners();
			_laboratoryNonInteractiveButton.onClick.RemoveAllListeners();
			_laboratoryNoOpponentButton.onClick.RemoveAllListeners();
			
			_containsError.interactable = true;

			if (_viewModel != null)
				_viewModel.CanStartNextTrial.ValueChanged -= OnCanStartNextTrialChanged;

			foreach (Transform child in _xrTrackerStatusContainer)
				Destroy(child.gameObject);
		}

		bool _shouldUpdate;
		ExperimentViewModel _viewModel;
	}
}
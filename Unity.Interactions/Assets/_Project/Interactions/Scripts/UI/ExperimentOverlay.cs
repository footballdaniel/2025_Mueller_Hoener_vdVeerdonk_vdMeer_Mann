using System;
using Interactions.Apps.ViewModels;
using Interactions.Domain.VideoRecorder;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interactions.UI
{
	public class ExperimentOverlay : UIScreen
	{
		[SerializeField] RawImage _cameraFeed;
		[SerializeField] TMP_Text _fpsText;
		[SerializeField] TMP_Text _xrStatusText;
		[SerializeField] Button _nextTrialButton;
		[SerializeField] Button _stopTrialButton;
		[SerializeField] Button _showDataButton;
		[SerializeField] Button _exitButton;
		[SerializeField] TMP_Text _progressText;
		[SerializeField] Slider _progressSlider;
		[SerializeField] XRTrackerStatus _xrTrackerStatusPrefab;
		[SerializeField] RectTransform _xrTrackerStatusContainer;

		void Update()
		{
			_fpsText.text = $"FPS: {Math.Round(1 / Time.deltaTime)} FPS Fixed: {Math.Round(1 / Time.fixedDeltaTime)}";

			_progressSlider.maxValue = _newProgress.MaxValue;
			_progressText.text = $"{_newProgress.Title}: {_newProgress.Task}";
			_progressSlider.value = _newProgress.Value;
			
			_cameraFeed.texture = _viewmodel.Frame;
		}

		public void Bind(ExperimentViewModel viewModel)
		{
			Unbind();

			_newProgress = new ProgressStatement();
			_viewmodel = viewModel;
			_cameraFeed.gameObject.SetActive(true);

			_nextTrialButton.onClick.AddListener(viewModel.NextTrial);
			_stopTrialButton.onClick.AddListener(viewModel.StopTrial);
			_showDataButton.onClick.AddListener(viewModel.ShowData);
			_exitButton.onClick.AddListener(viewModel.Exit);

			_stopTrialButton.interactable = false;

			viewModel.CanStartNextTrial.ValueChanged += OnCanStartNextTrialChanged;
			viewModel.Progress.ProgressChanged += OnProgressChanged;

			var headTracker = Instantiate(_xrTrackerStatusPrefab, _xrTrackerStatusContainer);
			var dominantFootTracker = Instantiate(_xrTrackerStatusPrefab, _xrTrackerStatusContainer);
			var nonDominantFootTracker = Instantiate(_xrTrackerStatusPrefab, _xrTrackerStatusContainer);
			var hipsTracker = Instantiate(_xrTrackerStatusPrefab, _xrTrackerStatusContainer);

			dominantFootTracker.Bind(viewModel.DominantFootTracker);
			headTracker.Bind(viewModel.HeadTracker);
			nonDominantFootTracker.Bind(viewModel.NonDominantFootTracker);
			hipsTracker.Bind(viewModel.DefenderHipsTracker);
		}

		void OnCanStartNextTrialChanged(bool canStart)
		{
			_stopTrialButton.interactable = !canStart;
			_nextTrialButton.interactable = canStart;
		}


		void OnProgressChanged(ProgressStatement progress)
		{
			_newProgress = progress;
		}

		void Unbind()
		{
			_nextTrialButton.onClick.RemoveAllListeners();
			_stopTrialButton.onClick.RemoveAllListeners();
			_showDataButton.onClick.RemoveAllListeners();
			_exitButton.onClick.RemoveAllListeners();

			if (_viewmodel != null)
			{
				_viewmodel.CanStartNextTrial.ValueChanged -= OnCanStartNextTrialChanged;
				_viewmodel.Progress.ProgressChanged -= OnProgressChanged;
			}

			foreach (Transform child in _xrTrackerStatusContainer)
				Destroy(child.gameObject);
		}

		ProgressStatement _newProgress;
		bool _shouldUpdate;
		ExperimentViewModel _viewmodel;
	}
}
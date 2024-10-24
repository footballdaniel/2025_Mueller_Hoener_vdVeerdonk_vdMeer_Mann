using System;
using App;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class ExperimentOverlay : MonoBehaviour
	{
		[SerializeField] TMP_Text _fpsText;
		[SerializeField] Button _nextTrialButton;
		[SerializeField] TMP_Text _progressText;
		[SerializeField] Slider _progressSlider;

		void Update()
		{
			_fpsText.text = $"FPS: {Math.Round(1 / Time.deltaTime)} FPS Fixed: {Math.Round(1 / Time.fixedDeltaTime)}";

			if (!_shouldUpdate)
				return;

			_progressSlider.maxValue = _newProgress.MaxValue;
			_progressText.text = $"{_newProgress.Title}: {_newProgress.Task}";
			_progressSlider.value = _newProgress.Value;
		}

		public void Set(ExperimentViewModel viewModel)
		{
			_nextTrialButton.onClick.AddListener(viewModel.NextTrial);
			
			viewModel.TrialStatusChanged += OnTrialStatusStatusChanged;
			viewModel.Progress.ProgressChanged += OnProgressChanged;
		}

		void OnTrialStatusStatusChanged(bool canStart)
		{
			_nextTrialButton.interactable = canStart;
		}


		void OnProgressChanged(ProgressStatement progress)
		{
			_shouldUpdate = true;
			_newProgress = progress;
		}

		ProgressStatement _newProgress;
		bool _shouldUpdate;
	}
}
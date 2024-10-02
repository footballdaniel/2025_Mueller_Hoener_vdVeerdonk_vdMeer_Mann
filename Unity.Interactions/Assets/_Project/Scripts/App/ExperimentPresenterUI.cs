using App;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class ExperimentPresenterUI : MonoBehaviour
	{
		[SerializeField] TMP_Text _fpsText;
		[SerializeField] Button _restartButton;
	
		void Update()
		{
			_fpsText.text = $"FPS: {1 / Time.fixedDeltaTime:0}";
		}

		public void Set(ExperimentViewModel viewModel)
		{
			_viewModel = viewModel;
			_viewModel.CanStartNextTrial.ValueChanged += EnableNextTrial;

			_restartButton.onClick.AddListener(_viewModel.NextTrial);
		}

		void EnableNextTrial(bool isEnabled)
		{
			_restartButton.interactable = isEnabled;
		}

		void OnDestroy()
		{
			_restartButton.onClick.RemoveAllListeners();
		}

		ExperimentViewModel _viewModel;
	}
}
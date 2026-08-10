using Interactions.Apps.ViewModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interactions.UI
{
	public class XRStatusUI : UIScreen
	{
		[SerializeField] TMP_Text _errorText;
		[SerializeField] Button _setUpButton;
		[SerializeField] TMP_Text _statusText;

		public void Bind(XRStatusViewModel viewModel)
		{
			_viewModel = viewModel;

			if (!viewModel.HasErrors())
			{
				viewModel.Startup();
				return;
			}

			_errorText.gameObject.SetActive(true);

			if (_setUpButton == null)
				return;

			_setUpButton.onClick.RemoveAllListeners();
			_setUpButton.onClick.AddListener(viewModel.StartSteamVRAndRetry);
			_setUpButton.gameObject.SetActive(true);
		}

		void Update()
		{
			if (_viewModel == null)
				return;

			if (_setUpButton != null)
				_setUpButton.interactable = !_viewModel.IsBusy;

			if (_statusText != null)
				_statusText.SetText(_viewModel.StatusMessage);
		}

		XRStatusViewModel _viewModel;
	}
}

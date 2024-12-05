using Interactions.Application.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace Interactions.UI
{
	public class InSituUI : UIScreen
	{
		[SerializeField] RawImage _cameraFeed;
		InSituTrialViewModel _viewModel;

		void Update()
		{
			if (_viewModel == null) return;
			
			_cameraFeed.texture = _viewModel.Frame;
		}

		public void Bind(InSituTrialViewModel viewModel)
		{
			_cameraFeed.gameObject.SetActive(true);
			_viewModel = viewModel;
		}
	}
}
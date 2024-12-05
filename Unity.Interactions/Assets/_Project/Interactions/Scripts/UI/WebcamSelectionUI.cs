using Interactions.Scripts.Application.ViewModels;
using Interactions.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Interactions.Scripts.UI
{
	public class WebcamSelectionUI : UIScreen
	{
		[SerializeField] WebcamConfigButton _webcamConfigButtonPrefab;
		[SerializeField] VerticalLayoutGroup _webcamConfigRoot;

		public void Set(WebcamSelectionViewModel viewModel)
		{
			foreach (var webcam in viewModel.Webcams)
			{
				var webcamElement = Instantiate(_webcamConfigButtonPrefab, _webcamConfigRoot.transform);
				webcamElement.Set(webcam);
				webcamElement.Button.onClick.AddListener(() => viewModel.Select(webcam));
			}
		}
	}
}
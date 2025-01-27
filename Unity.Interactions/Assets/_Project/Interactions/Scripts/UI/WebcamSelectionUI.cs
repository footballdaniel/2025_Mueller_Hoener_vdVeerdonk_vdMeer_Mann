using Interactions.Apps.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace Interactions.UI
{
	public class WebcamSelectionUI : UIScreen
	{
		[SerializeField] WebcamConfigButton _webcamConfigButtonPrefab;
		[SerializeField] VerticalLayoutGroup _webcamConfigRoot;

		public void Bind(WebcamSelectionViewModel viewModel)
		{
			foreach (var webcam in viewModel.Webcams)
			{
				var webcamElement = Instantiate(_webcamConfigButtonPrefab, _webcamConfigRoot.transform);
				webcamElement.Set(webcam);
				webcamElement.Button.onClick.AddListener(() => viewModel.Select(webcam));
			}
		}
		
		void OnDisable()
		{
			foreach (Transform child in _webcamConfigRoot.transform)
				Destroy(child.gameObject);
		}
	}
}
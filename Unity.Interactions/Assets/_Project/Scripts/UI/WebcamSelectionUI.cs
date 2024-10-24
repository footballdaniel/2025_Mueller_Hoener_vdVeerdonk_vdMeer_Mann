using App;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class WebcamSelectionUI : UIScreen
	{
		[SerializeField] UIWebcamConfigEntry _webcamConfigEntryPrefab;
		[SerializeField] VerticalLayoutGroup _webcamConfigRoot;

		public void Set(WebcamSelectionViewModel viewModel)
		{
			foreach (var webcam in viewModel.Webcams)
			{
				var webcamElement = Instantiate(_webcamConfigEntryPrefab, _webcamConfigRoot.transform);
				webcamElement.Set(webcam);
				webcamElement.Button.onClick.AddListener(() => viewModel.Select(webcam));
			}
		}
	}
}
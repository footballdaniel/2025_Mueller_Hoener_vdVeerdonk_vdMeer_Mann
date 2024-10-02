using App;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class WebcamSelectionUI : UIScreen
	{
		[SerializeField] UIWebcamConfigEntry _webcamConfigEntryPrefab;
		[SerializeField] VerticalLayoutGroup _webcamConfigRoot;
		
		public void Set(WebcamSelectionPresenter presenter)
		{
			foreach (var webcam in presenter.Webcams)
			{
				var webcamElement = Instantiate(_webcamConfigEntryPrefab, _webcamConfigRoot.transform);
				webcamElement.Set(webcam);
				webcamElement.Button.onClick.AddListener(() => presenter.Select(webcam));
			}
		}
	}
}
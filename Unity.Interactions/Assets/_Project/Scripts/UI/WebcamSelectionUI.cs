using App;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class WebcamSelectionUI : MonoBehaviour
	{
		[SerializeField] UIWebcamConfigEntry _webcamConfigEntryPrefab;
		[SerializeField] VerticalLayoutGroup _webcamConfigRoot;
		
		public void Set(WebcamSelectionPresenter presenter)
		{
			foreach (var webcam in presenter.WebcamRepository)
			{
				var webcamConfig = Instantiate(_webcamConfigEntryPrefab, _webcamConfigRoot.transform);
				webcamConfig.Set(webcam);
				webcamConfig.Button.onClick.AddListener(() => presenter.Select(webcam));
			}
		}
	}
}
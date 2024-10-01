using App;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class WebcamSelectionUI : MonoBehaviour
	{
		[SerializeField] Button _webcamConfigPrefab;
		[SerializeField] VerticalLayoutGroup _webcamConfigRoot;
		
		public void Set(WebcamSelectionPresenter presenter)
		{
			foreach (var webcam in presenter.AvailableWebCams)
			{
				var webcamConfig = Instantiate(_webcamConfigPrefab, _webcamConfigRoot.transform);
				webcamConfig.GetComponentInChildren<TMP_Text>().text = webcam.DeviceName + " " + webcam.FrameRate + "fps" + " " + webcam.Width + "x" + webcam.Height;
				webcamConfig.onClick.AddListener(() => presenter.Select(webcam));
			}
		}
	}
}
using _Project.Scripts.App;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
	[SerializeField] TMP_Text _fpsText;
	[SerializeField] Button _restartButton;
	
	// webcam selection
	[SerializeField] Button _webcamConfigPrefab;
	[SerializeField] VerticalLayoutGroup _webcamConfigRoot;
	
	
	void Update()
	{
		_fpsText.text = $"FPS: {1 / Time.fixedDeltaTime:0}";
	}

	public void Set(ExperimentPresenter presenter)
	{
		_presenter = presenter;
		_presenter.CanStartNextTrial.ValueChanged += EnableNextTrial;
		
		_restartButton.onClick.AddListener(_presenter.NextTrial);
		
		foreach (var webcam in presenter.AvailableWebCams)
		{
			var webcamConfig = Instantiate(_webcamConfigPrefab, _webcamConfigRoot.transform);
			webcamConfig.GetComponentInChildren<TMP_Text>().text = webcam.DeviceName + " " + webcam.FrameRate + "fps" + " " + webcam.Width + "x" + webcam.Height;
			webcamConfig.onClick.AddListener(() => presenter.Select(webcam));
		}
	}

	void EnableNextTrial(bool isEnabled)
	{
		_restartButton.interactable = isEnabled;
	}

	void OnDestroy()
	{
		_restartButton.onClick.RemoveAllListeners();
	}

	ExperimentPresenter _presenter;
}
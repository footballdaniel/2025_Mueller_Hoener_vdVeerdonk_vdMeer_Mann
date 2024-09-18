using _Project.Scripts.App;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
	[SerializeField] TMP_Text _fpsText;
	[SerializeField] Button _restartButton;

	void OnEnable()
	{
		_restartButton.onClick.AddListener(_presenter.NextTrial);
	}

	void Update()
	{
		_fpsText.text = $"FPS: {1 / Time.fixedDeltaTime:0}";
	}

	public void Set(ExperimentPresenter presenter)
	{
		_presenter = presenter;
		_presenter.CanStartNextTrial.ValueChanged += EnableNextTrial;
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
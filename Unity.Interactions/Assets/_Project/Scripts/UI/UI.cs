using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
	[SerializeField] TMP_Text _fpsText;
	[SerializeField] Button _restartButton;

	void OnEnable()
	{
		_restartButton.onClick.AddListener(() => AppEvents.RestartRequested?.Invoke());
	}

	void OnDestroy()
	{
		_restartButton.onClick.RemoveAllListeners();
	}

	void Update()
	{
		_fpsText.text = $"FPS: {1 / Time.fixedDeltaTime:0}";
	}
}

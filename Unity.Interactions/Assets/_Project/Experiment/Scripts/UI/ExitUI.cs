using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ExitUI : MonoBehaviour
{
	[SerializeField] Button _exitButton;

	void OnEnable()
	{
		_exitButton.onClick.AddListener(Exit);
	}


	void Exit()
	{
#if UNITY_EDITOR
		if (Application.isEditor)
			EditorApplication.isPlaying = false;
		else
#endif
			Application.Quit();
	}
}
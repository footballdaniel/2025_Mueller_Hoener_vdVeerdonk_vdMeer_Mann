using TMPro;
using UnityEngine;

public class FPSOverlayUI : MonoBehaviour
{
	[SerializeField] TMP_Text _fpsText;
	
	void Update()
	{
		_fpsText.text = $"FPS: {1 / Time.fixedDeltaTime:0}";
	}
}

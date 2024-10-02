using Domain.VideoRecorder;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWebcamConfigEntry : MonoBehaviour
{
	[field: SerializeReference] public  Button Button { get; private set; }
	[SerializeField] TMP_Text _text;

	public void Set(WebCamConfiguration config)
	{
		_text.text = config.DeviceName + " " + " " + config.Width + "x" + config.Height;
	}
}

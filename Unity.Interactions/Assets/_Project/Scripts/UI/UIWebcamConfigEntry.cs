using Domain.VideoRecorder;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWebcamConfigEntry : MonoBehaviour
{
	[field: SerializeReference] public Button Button { get; private set; }
	[SerializeField] TMP_Text _text;

	public void Set(IWebcamRecorder recorder)
	{
		_text.text = recorder.Info.DeviceName + " " + " " + recorder.Info.Width + "x" + recorder.Info.Height;
	}
}
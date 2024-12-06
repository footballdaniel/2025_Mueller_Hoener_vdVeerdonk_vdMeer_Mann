using Interactions.Domain.VideoRecorder;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interactions.UI
{
	public class WebcamConfigButton : MonoBehaviour
	{
		[field: SerializeReference] public Button Button { get; private set; }
		[SerializeField] TMP_Text _text;

		public void Set(IWebcamRecorder recorder)
		{
			_text.text = recorder.Specs.DeviceName + " " + " " + recorder.Specs.Width + "x" + recorder.Specs.Height;
		}
	}

}
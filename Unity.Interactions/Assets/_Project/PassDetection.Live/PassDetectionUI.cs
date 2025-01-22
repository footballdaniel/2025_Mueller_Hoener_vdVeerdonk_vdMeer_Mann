using UnityEngine;
using UnityEngine.UIElements;

public class PassDetectionUI : MonoBehaviour
{
	[SerializeField] UIDocument _uiDocument;
	_Project.PassDetection.Live.PassDetection _app;
	Slider _slider;
	Label _passCountLabel;

	public void Bind(_Project.PassDetection.Live.PassDetection app)
	{
		_app = app;
		_slider = _uiDocument.rootVisualElement.Q<Slider>();
		_passCountLabel = _uiDocument.rootVisualElement.Q<Label>("pass-count");
	}


	void Update()
	{
		_slider.value = _app.Prediction;
		_passCountLabel.text = _app.PassCount.ToString();
	}
}
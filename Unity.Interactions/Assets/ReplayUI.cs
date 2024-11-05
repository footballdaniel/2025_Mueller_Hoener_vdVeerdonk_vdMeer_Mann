using UnityEngine;
using UnityEngine.UI;

public class ReplayUI : MonoBehaviour
{
	[SerializeField] Slider _timeSlider;
	[SerializeField] Button _playPauseButton;
	
	public void Set(ReplayApp app)
	{
		_timeSlider.maxValue = app.NumberOfFrames - 1;
		_timeSlider.onValueChanged.AddListener(value => app.ShowFrame((int)value));
		_playPauseButton.onClick.AddListener(app.TogglePlayPause);
		
		app.TogglePlayPause();
	}
}

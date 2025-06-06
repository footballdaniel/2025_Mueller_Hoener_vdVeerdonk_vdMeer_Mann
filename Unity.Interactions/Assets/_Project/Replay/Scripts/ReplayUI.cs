using Replay.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Replay.Scripts
{
	public class ReplayUI : MonoBehaviour
	{
		[SerializeField] Slider _timeSlider;
		[SerializeField] Button _playPauseButton;
		ReplayApp _app;

		public void Set(ReplayApp app)
		{
			_timeSlider.maxValue = app.NumberOfFrames - 1;
			_timeSlider.onValueChanged.AddListener(value => app.ShowFrame((int)value));
			_playPauseButton.onClick.AddListener(app.TogglePlayPause);

			_app = app;
			app.TogglePlayPause();
		}

		void Update()
		{
			_timeSlider.value = _app.CurrentFrameIndex;
		}


	}
}
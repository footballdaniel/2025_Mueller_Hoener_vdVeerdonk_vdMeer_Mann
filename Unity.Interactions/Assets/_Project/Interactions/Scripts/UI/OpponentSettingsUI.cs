using Interactions.Apps.ViewModels;
using UnityEngine;

namespace Interactions.UI
{
	public class OpponentSettingsUI : UIScreen
	{
		[SerializeField] SettingSlider _interPersonalDistanceSlider;
		[SerializeField] SettingSlider _bodyInformationSlider;
		[SerializeField] SettingSlider _footInformationSlider;
		[SerializeField] SettingSlider _reactionTimeBodySlider;
		[SerializeField] SettingSlider _reactionTimeFootSlider;
		[SerializeField] SettingSlider _accelerationSlider;
		[SerializeField] SettingSlider _goalDistanceSlider;

		public void Bind(OpponentSettingsViewModel settingsViewModel)
		{
			_interPersonalDistanceSlider.Slider.onValueChanged.AddListener(settingsViewModel.ChangeInterpersonalDistance);
			_interPersonalDistanceSlider.Bind("Weight IPD", settingsViewModel.InterpersonalDistance, 0, 10);

			_bodyInformationSlider.Slider.onValueChanged.AddListener(settingsViewModel.ChangeBodyInformationWeight);
			_bodyInformationSlider.Bind("Weight Body Info", settingsViewModel.BodyInformation, 0.01f, 1);

			_footInformationSlider.Slider.onValueChanged.AddListener(settingsViewModel.ChangeFootInformation);
			_footInformationSlider.Bind("Weight Foot Info", settingsViewModel.FootInformation, 0.01f, 1);

			_reactionTimeBodySlider.Slider.onValueChanged.AddListener(settingsViewModel.ChangeReactionTime);
			_reactionTimeBodySlider.Bind("Reaction Time", settingsViewModel.ReactionTime, 0f, 2);

			_reactionTimeFootSlider.Slider.onValueChanged.AddListener(settingsViewModel.ChangeReactionTimeFoot);
			_reactionTimeFootSlider.Bind("Reaction Time Foot", settingsViewModel.ReactionTimeFoot, 0f, 2);

			_accelerationSlider.Slider.onValueChanged.AddListener(settingsViewModel.ChangeAcceleration);
			_accelerationSlider.Bind("Acceleration", settingsViewModel.Acceleration, 0f, 20f);

			_goalDistanceSlider.Slider.onValueChanged.AddListener(settingsViewModel.ChangeGoalDistance);
			_goalDistanceSlider.Bind("Goal Distance", settingsViewModel.GoalDistance, 0, 5);
		}
	}
}
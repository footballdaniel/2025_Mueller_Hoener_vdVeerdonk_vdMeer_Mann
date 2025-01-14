using Interactions.Apps.ViewModels;
using Interactions.UI;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : UIScreen
{
	[SerializeField] SettingSlider _interPersonalDistanceSlider;
	[SerializeField] SettingSlider _bodyInformationSlider;
	[SerializeField] SettingSlider _footInformationSlider;
	[SerializeField] SettingSlider _reactionTimeBodySlider;
	[SerializeField] SettingSlider _reactionTimeFootSlider;
	[SerializeField] SettingSlider _accelerationSlider;
	[SerializeField] SettingSlider _goalDistanceSlider;
	
	public void Bind(OpponentViewModel viewModel)
	{
		_interPersonalDistanceSlider.Slider.onValueChanged.AddListener(viewModel.ChangeInterpersonalDistance);
		_interPersonalDistanceSlider.Bind("Weight IPD", viewModel.InterpersonalDistance, 0, 10);
		
		_bodyInformationSlider.Slider.onValueChanged.AddListener(viewModel.ChangeBodyInformationWeight);
		_bodyInformationSlider.Bind("Weight Body Info", viewModel.BodyInformation, 0.01f, 1);
		
		_footInformationSlider.Slider.onValueChanged.AddListener(viewModel.ChangeFootInformation);
		_footInformationSlider.Bind("Weight Foot Info", viewModel.FootInformation, 0.01f, 1);
		
		_reactionTimeBodySlider.Slider.onValueChanged.AddListener(viewModel.ChangeReactionTime);
		_reactionTimeBodySlider.Bind("Reaction Time", viewModel.ReactionTime, 0f, 2);
		
		_reactionTimeFootSlider.Slider.onValueChanged.AddListener(viewModel.ChangeReactionTimeFoot);
		_reactionTimeFootSlider.Bind("Reaction Time Foot", viewModel.ReactionTimeFoot, 0f, 2);
		
		_accelerationSlider.Slider.onValueChanged.AddListener(viewModel.ChangeAcceleration);
		_accelerationSlider.Bind("Acceleration", viewModel.Acceleration, 0f, 20f);
		
		_goalDistanceSlider.Slider.onValueChanged.AddListener(viewModel.ChangeGoalDistance);
		_goalDistanceSlider.Bind("Goal Distance", viewModel.GoalDistance, 0, 5);
		
		
	}
}

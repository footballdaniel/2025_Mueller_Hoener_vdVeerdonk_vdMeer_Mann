using Interactions.Apps.ViewModels;
using Interactions.UI;
using UnityEngine;

public class SettingsUI : UIScreen
{
	[SerializeField] SettingSlider _interPersonalDistanceSlider;
	[SerializeField] SettingSlider _reactionTimeSlider;
	[SerializeField] SettingSlider _accelerationSlider;
	[SerializeField] SettingSlider _goalDistanceSlider;
	
	public void Bind(OpponentViewModel viewModel)
	{
		_interPersonalDistanceSlider.Slider.onValueChanged.AddListener(viewModel.ChangeInterpersonalDistance);
		_interPersonalDistanceSlider.Bind("IPD", viewModel.InterpersonalDistance, 2, 10);
		
		_reactionTimeSlider.Slider.onValueChanged.AddListener(viewModel.ChangeReactionTime);
		_reactionTimeSlider.Bind("Reaction Time", viewModel.ReactionTime, 0.1f, 2);
		
		_accelerationSlider.Slider.onValueChanged.AddListener(viewModel.ChangeAcceleration);
		_accelerationSlider.Bind("Acceleration", viewModel.Acceleration, 0f, 20f);
		
		_goalDistanceSlider.Slider.onValueChanged.AddListener(viewModel.ChangeGoalDistance);
		_goalDistanceSlider.Bind("Goal Distance", viewModel.GoalDistance, 0, 5);
	}
}

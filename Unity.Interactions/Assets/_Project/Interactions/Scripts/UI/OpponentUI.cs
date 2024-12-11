using Interactions.Apps.ViewModels;
using Interactions.UI;
using UnityEngine;

public class OpponentUI : UIScreen
{
	[SerializeField] OpponentSlider _interPersonalDistanceSlider;
	[SerializeField] OpponentSlider _reactionTimeSlider;
	[SerializeField] OpponentSlider _accelerationSlider;
	[SerializeField] OpponentSlider _ballSpeedSlider;
	
	public void Bind(OpponentViewModel viewModel)
	{
		_interPersonalDistanceSlider.Slider.onValueChanged.AddListener(viewModel.ChangeInterpersonalDistance);
		_interPersonalDistanceSlider.Bind("IPD", viewModel.InterpersonalDistance, 2, 10);
		
		_reactionTimeSlider.Slider.onValueChanged.AddListener(viewModel.ChangeReactionTime);
		_reactionTimeSlider.Bind("Reaction Time", viewModel.ReactionTime, 0.1f, 2);
		
		_accelerationSlider.Slider.onValueChanged.AddListener(viewModel.ChangeAcceleration);
		_accelerationSlider.Bind("Acceleration", viewModel.Acceleration, 0f, 20f);
		
		// todo
	}
}

using System.Collections.Generic;

namespace Interactions.Apps.ViewModels
{
	public class OpponentSettingsViewModel
	{

		public OpponentSettingsViewModel(App app)
		{
			_app = app;
		}

		public IEnumerable<SettingDescriptor> GetDescriptors()
		{
			return new[]
			{
				new SettingDescriptor("Weight IPD", InterpersonalDistance, 0f, 10f, ChangeInterpersonalDistance),
				new SettingDescriptor("Weight Body Info", BodyInformation, 0.01f, 1f, ChangeBodyInformationWeight),
				new SettingDescriptor("Weight Foot Info", FootInformation, 0.01f, 1f, ChangeFootInformation),
				new SettingDescriptor("Reaction Time", ReactionTime, 0f, 2f, ChangeReactionTime),
				new SettingDescriptor("Reaction Time Foot", ReactionTimeFoot, 0f, 2f, ChangeReactionTimeFoot),
				new SettingDescriptor("Acceleration", Acceleration, 0f, 20f, ChangeAcceleration),
				new SettingDescriptor("Goal Distance", GoalDistance, 0f, 5f, ChangeGoalDistance),
			};
		}

		public float InterpersonalDistance
		{
			get => _app.Experiment.InterPersonalDistance;
			set => ChangeInterpersonalDistance(value);
		}

		public float ReactionTime
		{
			get => _app.Experiment.OpponentReactionTimeBody;
			set => ChangeReactionTime(value);
		}

		public float Acceleration
		{
			get => _app.Experiment.OpponentAcceleration;
			set => ChangeAcceleration(value);
		}

		public float GoalDistance
		{
			get => _app.Experiment.DistanceBetweenGoals;
			set => ChangeGoalDistance(value);
		}

		public float BodyInformation
		{
			get => _app.Experiment.BodyInformationWeight;
			set => ChangeBodyInformationWeight(value);
		}

		public float FootInformation
		{
			get => _app.Experiment.FootInformationWeight;
			set => _app.Experiment.Opponent.ChangeFootInformation(value);
		}

		public float ReactionTimeFoot
		{
			get => _app.Experiment.OpponentReactionTimeFoot;
			set => _app.Experiment.Opponent.ChangeReactionTimeBody(value);
		}

		public void ChangeAcceleration(float newAcceleration)
		{
			_app.Experiment.OpponentAcceleration = newAcceleration;
			if (_app.Experiment.Opponent != null)
				_app.Experiment.Opponent.ChangeAcceleration(newAcceleration);
			_app.Config.OpponentAcceleration = newAcceleration;
		}

		public void ChangeBodyInformationWeight(float arg0)
		{
			_app.Experiment.BodyInformationWeight = arg0;
			if (_app.Experiment.Opponent != null)
				_app.Experiment.Opponent.ChangeBodyInformationWeight(arg0);
			_app.Config.BodyInformationWeight = arg0;
		}

		public void ChangeFootInformation(float arg0)
		{
			_app.Experiment.FootInformationWeight = arg0;
			if (_app.Experiment.Opponent != null)
				_app.Experiment.Opponent.ChangeFootInformation(arg0);
			_app.Config.FootInformationWeight = arg0;
		}

		public void ChangeGoalDistance(float newDistance)
		{
			_app.Experiment.DistanceBetweenGoals = newDistance;
			_app.Experiment.LeftGoal.PlaceWithDistance(newDistance / 2f);
			_app.Experiment.RightGoal.PlaceWithDistance(newDistance / 2f);
			_app.Config.DistanceBetweenGoals = newDistance;
		}

		public void ChangeInterpersonalDistance(float newDistance)
		{
			_app.Experiment.InterPersonalDistance = newDistance;
			if (_app.Experiment.Opponent != null)
				_app.Experiment.Opponent.ChangeInterpersonalDistance(newDistance);
			_app.Config.InterPersonalDistance = newDistance;
		}

		public void ChangeReactionTime(float newReactionTime)
		{
			_app.Experiment.OpponentReactionTimeBody = newReactionTime;
			if (_app.Experiment.Opponent != null)
				_app.Experiment.Opponent.ChangeReactionTimeBody(newReactionTime);
			_app.Config.OpponentReactionTimeBody = newReactionTime;
		}

		readonly App _app;

		public void ChangeReactionTimeFoot(float arg0)
		{
			_app.Experiment.OpponentReactionTimeFoot = arg0;
			if (_app.Experiment.Opponent != null)
				_app.Experiment.Opponent.ChangeReactionTimeFoot(arg0);
			_app.Config.OpponentReactionTimeFoot = arg0;
		}
	}
}
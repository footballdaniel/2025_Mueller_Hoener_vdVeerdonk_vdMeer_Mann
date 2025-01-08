namespace Interactions.Apps.ViewModels
{
	public class OpponentViewModel
	{

		public OpponentViewModel(App app)
		{
			_app = app;
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
			_app.Experiment.Opponent.ChangeAcceleration(newAcceleration);
		}

		public void ChangeBodyInformationWeight(float arg0)
		{
			_app.Experiment.Opponent.ChangeBodyInformationWeight(arg0);
		}

		public void ChangeFootInformation(float arg0)
		{
			_app.Experiment.Opponent.ChangeFootInformation(arg0);
		}

		public void ChangeGoalDistance(float newDistance)
		{
			_app.Experiment.LeftGoal.PlaceWithDistance(newDistance / 2f);
			_app.Experiment.RightGoal.PlaceWithDistance(newDistance / 2f);
		}

		public void ChangeInterpersonalDistance(float newDistance)
		{
			_app.Experiment.InterPersonalDistance = newDistance;
			_app.Experiment.Opponent.ChangeInterpersonalDistance(newDistance);
		}

		public void ChangeReactionTime(float newReactionTime)
		{
			_app.Experiment.OpponentReactionTimeBody = newReactionTime;
			_app.Experiment.Opponent.ChangeReactionTimeBody(newReactionTime);
		}

		readonly App _app;

		public void ChangeReactionTimeFoot(float arg0)
		{
			_app.Experiment.OpponentReactionTimeFoot = arg0;
			_app.Experiment.Opponent.ChangeReactionTimeFoot(arg0);
		}
	}
}
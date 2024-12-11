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
			get => _app.Experiment.OpponentReactionTime;
			set => ChangeReactionTime(value);
		}

		public float Acceleration
		{
			get => _app.Experiment.OpponentAcceleration;
			set => ChangeAcceleration(value);
		}

		public void ChangeAcceleration(float newAcceleration)
		{
			_app.Experiment.OpponentAcceleration = newAcceleration;
			_app.Experiment.Opponent.ChangeAcceleration(newAcceleration);
		}

		public void ChangeInterpersonalDistance(float newDistance)
		{
			_app.Experiment.InterPersonalDistance = newDistance;
			_app.Experiment.Opponent.ChangeInterpersonalDistance(newDistance);
		}

		public void ChangeReactionTime(float newReactionTime)
		{
			_app.Experiment.OpponentReactionTime = newReactionTime;
			_app.Experiment.Opponent.ChangeReactionTime(newReactionTime);
		}

		readonly App _app;
		Opponent _opponent;
	}
}
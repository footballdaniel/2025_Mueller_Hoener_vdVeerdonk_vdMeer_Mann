using Interactions.Domain;

namespace Interactions.Application.States
{
	public class StartExperiment : State
	{

		public StartExperiment(App app) : base(app)
		{
		}

		public void ConditionSelected(ExperimentalCondition condition)
		{
			_app.ExperimentalCondition = condition;
			_app.Transitions.StartExperiment.Execute();
		}


		public override void Enter()
		{
			_app.UI.ExperimentOverlay.Bind(_app.ExperimentViewModel);
			_app.UI.ExperimentSetupUI.Bind(this);
			_app.UI.ExperimentSetupUI.Show();
		}

		public override void Exit()
		{
			_app.UI.ExperimentSetupUI.Hide();
		}


		public override void Tick()
		{
			_app.Transitions.SelectWebcam.Execute();
		}

		public void DominantFootSelected(Side side)
		{
			_app.DominantFootSide = side;
		}
	}
}
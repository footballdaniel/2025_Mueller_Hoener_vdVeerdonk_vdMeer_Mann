using Interactions.Scripts.Application.States;

namespace Interactions.Scripts.Application
{
	public class ConditionSelection : State
	{
		public ConditionSelection(App app) : base(app)
		{
			
		}
		
		public override void Enter()
		{
			_app.UI.ConditionSelectionUI.Bind(this);
		}

		public void ConditionSelected(ExperimentalCondition condition)
		{
			_app.ExperimentalCondition = condition;
			_app.Transitions.Init.Execute();
		}
	}
}
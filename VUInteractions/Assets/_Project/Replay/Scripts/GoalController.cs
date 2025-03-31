using System.Collections.Generic;
using System.Linq;

namespace _Project.Replay.Scripts
{
	internal class GoalController
	{
		readonly Goal _leftGoal;
		readonly List<FrameEvent> _frameEvents;
		readonly Goal _rightGoal;

		public GoalController(List<FrameEvent> frameEvents, Goal leftGoal, Goal rightGoal)
		{
			_frameEvents = frameEvents;
			_leftGoal = leftGoal;
			_rightGoal = rightGoal;
		}

		public void Tick(int frameIndex)
		{
			var currentEvent = _frameEvents.LastOrDefault(e => e.FrameIndex <= frameIndex);

			if (currentEvent == null)
			{
				_leftGoal.NoFeedback();
				_rightGoal.NoFeedback();
				return;
			}

			if (currentEvent.EventType == EventType.Pass)
			{
				if (currentEvent.GoalDirection == GoalDirection.Left)
					_leftGoal.Score();
				else if (currentEvent.GoalDirection == GoalDirection.Right)
					_rightGoal.Score();
			}
			else
			{
				if (currentEvent.GoalDirection == GoalDirection.Left)
					_leftGoal.Miss();
				else if (currentEvent.GoalDirection == GoalDirection.Right)
					_rightGoal.Miss();
			}
		}
	}
}
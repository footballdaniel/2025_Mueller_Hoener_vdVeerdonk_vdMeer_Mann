using Interactions.Domain.Feet;
using Interactions.Domain.Goals;
using UnityEngine;

namespace Interactions.Domain
{

	public class PassCorrector : IPassCorrector
	{

		public PassCorrector(DominantFoot dominantFoot, RightGoal rightGoal, LeftGoal leftGoal)
		{
			_dominantFoot = dominantFoot;
			_rightGoal = rightGoal;
			_leftGoal = leftGoal;
		}

		public Pass Correct(Pass pass, Vector3 referencePosition)
		{
			var passPosition = new Vector3(_dominantFoot.transform.position.x, 0.25f, _dominantFoot.transform.position.z);
			var isOpponentToLeftOfUser = referencePosition.z < _dominantFoot.transform.position.z;
			var optimalGoalPosition = isOpponentToLeftOfUser ? _rightGoal.transform.position : _leftGoal.transform.position;
			var correctedPassDirection = Vector3.Lerp(pass.Direction, optimalGoalPosition - passPosition, 0.1f).normalized;

			return new Pass(pass.Speed, passPosition, correctedPassDirection);
		}

		readonly DominantFoot _dominantFoot;

		readonly Experiment _experiment;
		readonly LeftGoal _leftGoal;
		readonly RightGoal _rightGoal;
	}
}
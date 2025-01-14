using Interactions.Domain.Feet;
using Interactions.Domain.Goals;
using Interactions.Domain.Opponents;
using UnityEngine;

namespace Interactions.Domain
{
	public class PassCorrector
	{

		public PassCorrector(DominantFoot dominantFoot, RightGoal rightGoal, LeftGoal leftGoal)
		{
			_dominantFoot = dominantFoot;
			_rightGoal = rightGoal;
			_leftGoal = leftGoal;
		}

		readonly Experiment _experiment;
		readonly DominantFoot _dominantFoot;
		readonly RightGoal _rightGoal;
		readonly LeftGoal _leftGoal;

		public Pass Correct(Pass pass, Vector3 referencePosition)
		{
			var passPosition = new Vector3(_dominantFoot.transform.position.x, 0.25f, _dominantFoot.transform.position.z);
			var isOpponentToLeftOfUser = referencePosition.z < _dominantFoot.transform.position.z;
			var optimalGoalPosition = isOpponentToLeftOfUser ? _rightGoal.transform.position : _leftGoal.transform.position;
			var correctedPassDirection = Vector3.Lerp(pass.Direction, optimalGoalPosition - passPosition, 0.1f).normalized;
			
			return new Pass(pass.Speed, passPosition, correctedPassDirection);

		}
	}
}
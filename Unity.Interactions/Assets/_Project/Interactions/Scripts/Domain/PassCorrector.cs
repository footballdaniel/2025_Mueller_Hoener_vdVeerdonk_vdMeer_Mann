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
			var passPosition = CorrectedPosition(pass, referencePosition, out var correctedPassDirection);
			var correctedKickVelocity = CorrectVelocity(pass);

			return new Pass(correctedKickVelocity, passPosition, correctedPassDirection);
		}

		Vector3 CorrectedPosition(Pass pass, Vector3 referencePosition, out Vector3 correctedPassDirection)
		{
			var passPosition = new Vector3(_dominantFoot.transform.position.x, 0.25f, _dominantFoot.transform.position.z);
			var isOpponentToLeftOfUser = referencePosition.z < _dominantFoot.transform.position.z;
			var optimalGoalPosition = isOpponentToLeftOfUser ? _leftGoal.transform.position : _rightGoal.transform.position;
			correctedPassDirection = Vector3.Lerp(pass.Direction, optimalGoalPosition - passPosition, 0.1f).normalized;
			return passPosition;
		}

		static float CorrectVelocity(Pass pass)
		{
			var optimalPassVelocity = 10f;
			var correctedKickVelocity = Mathf.Lerp(pass.KickVelocity, optimalPassVelocity, 0.5f);
			return correctedKickVelocity;
		}

		readonly DominantFoot _dominantFoot;

		readonly Experiment _experiment;
		readonly LeftGoal _leftGoal;
		readonly RightGoal _rightGoal;
	}
}
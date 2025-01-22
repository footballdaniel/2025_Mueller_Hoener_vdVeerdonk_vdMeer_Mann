using Interactions.Domain.Feet;
using Interactions.Domain.Goals;
using UnityEngine;

namespace Interactions.Domain
{

	public class PassCorrector : IPassCorrector
	{

		public PassCorrector(User user, RightGoal rightGoal, LeftGoal leftGoal)
		{
			_rightGoal = rightGoal;
			_leftGoal = leftGoal;
			_user = user;
		}

		public Pass Correct(Pass pass, Vector3 referencePosition)
		{
			var passPosition = CorrectedPosition(pass, referencePosition, out var correctedPassDirection);
			var correctedKickVelocity = CorrectVelocity(pass);

			return new Pass(correctedKickVelocity, passPosition, correctedPassDirection);
		}

		Vector3 CorrectedPosition(Pass pass, Vector3 referencePosition, out Vector3 correctedPassDirection)
		{
			var dominantFoot = _user.DominantFoot;
			var passPosition = new Vector3(dominantFoot.transform.position.x, 0.25f, dominantFoot.transform.position.z);
			var isOpponentToLeftOfUser = referencePosition.z < dominantFoot.transform.position.z;
			var optimalGoalPosition = isOpponentToLeftOfUser ? _leftGoal.transform.position : _rightGoal.transform.position;
			correctedPassDirection = Vector3.Lerp(pass.Direction, optimalGoalPosition - passPosition, 0.1f).normalized;
			return passPosition;
		}

		static float CorrectVelocity(Pass pass)
		{
			var optimalPassVelocity = 15f;
			var correctedKickVelocity = Mathf.Lerp(pass.KickVelocity, optimalPassVelocity, 0.5f);
			return correctedKickVelocity;
		}


		readonly Experiment _experiment;
		readonly LeftGoal _leftGoal;
		readonly RightGoal _rightGoal;
		readonly User _user;
	}
}
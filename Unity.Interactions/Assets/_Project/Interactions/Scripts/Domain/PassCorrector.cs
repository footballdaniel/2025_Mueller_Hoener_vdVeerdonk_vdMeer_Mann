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
			CorrectedPosition(referencePosition, out var correctedPassDirection);
			var correctedKickVelocity = CorrectVelocity(pass);

			return new Pass(correctedKickVelocity, pass.Position, correctedPassDirection);
		}

		void CorrectedPosition(Vector3 referencePosition, out Vector3 correctedPassDirection)
		{
			var dominantFoot = _user.DominantFoot;
			var passPosition = new Vector3(dominantFoot.transform.position.x, 0.25f, dominantFoot.transform.position.z);
			var isOpponentToLeftOfUser = referencePosition.z < dominantFoot.transform.position.z;
			var optimalGoalPosition = isOpponentToLeftOfUser ? _leftGoal.transform.position : _rightGoal.transform.position;
			
			// make pass go towards goal, and have a random offset around goal
			var lateralNoise = 0f;
			if (isOpponentToLeftOfUser)
				lateralNoise += Random.Range(-1f, 0.25f);
			else
				lateralNoise += Random.Range(1f, -0.25f);
			var optimalGoalPositionWithNoise = new Vector3(optimalGoalPosition.x, optimalGoalPosition.y, optimalGoalPosition.z + lateralNoise);
			
			correctedPassDirection = (optimalGoalPositionWithNoise - passPosition).normalized;
			// correctedPassDirection = Vector3.Lerp(pass.Direction, optimalGoalPosition - passPosition, 0.1f).normalized;
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
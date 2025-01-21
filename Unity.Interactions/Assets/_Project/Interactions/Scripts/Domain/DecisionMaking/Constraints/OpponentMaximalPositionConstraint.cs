using UnityEngine;

namespace Interactions.Domain.DecisionMaking.Constraints
{
	public class OpponentMaximalPositionConstraint
	{
		readonly float _minXPosition;

		public OpponentMaximalPositionConstraint(float minXPosition)
		{
			_minXPosition = minXPosition;
		}
			
		
		
		public Vector3 Constrain(Vector3 translation)
		{
			if (translation.x < _minXPosition)
				translation.x = _minXPosition;
			
			return translation;
		}
	}
}
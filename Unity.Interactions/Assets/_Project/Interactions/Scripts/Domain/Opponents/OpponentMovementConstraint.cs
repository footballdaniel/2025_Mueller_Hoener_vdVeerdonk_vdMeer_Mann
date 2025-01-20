using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class OpponentMovementConstraint
	{
		readonly float _minXPosition;

		public OpponentMovementConstraint(float minXPosition)
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
using Interactions.Domain.Feet;
using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class FootInformationSource : IInformationSource
	{

		public FootInformationSource(IPercept footPercept)
		{
			_footPercept = footPercept;
		}

		public Vector3 TargetPosition()
		{
			var pos = _footPercept.Perceive();
			return new Vector3(pos.x, 0, pos.y);
		}

		public float TargetRotationY()
		{
			return 0f;
		}

		public float Weight { get; set; }

		readonly DominantFoot _userDominantFoot;
		readonly NonDominantFoot _userNonDominantFoot;
		readonly IPercept _footPercept;
	}
}
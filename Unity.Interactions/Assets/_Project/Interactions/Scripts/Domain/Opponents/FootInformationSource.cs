using Interactions.Domain.Feet;
using UnityEngine;

namespace Interactions.Domain.Opponents
{
	internal class FootInformationSource : IInformationSource
	{

		public FootInformationSource(DominantFoot userDominantFoot, NonDominantFoot userNonDominantFoot)
		{
			_userDominantFoot = userDominantFoot;
			_userNonDominantFoot = userNonDominantFoot;
		}

		public Vector3 TargetPosition()
		{
			var dominantFootVelocityZ = _userDominantFoot.Velocity.z;
			var nonDominantFootVelocityZ = _userNonDominantFoot.Velocity.z;
			var combinedVelocityZ = dominantFootVelocityZ + nonDominantFootVelocityZ;

			return new Vector3(0, 0, combinedVelocityZ);
		}

		public float TargetRotationY()
		{
			return 0f;
		}

		public float Weight { get; set; }

		readonly DominantFoot _userDominantFoot;
		readonly NonDominantFoot _userNonDominantFoot;
	}
}
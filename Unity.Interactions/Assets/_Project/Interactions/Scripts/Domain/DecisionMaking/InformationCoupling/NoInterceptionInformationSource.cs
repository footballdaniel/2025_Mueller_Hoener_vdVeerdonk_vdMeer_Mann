using UnityEngine;

namespace Interactions.Domain.DecisionMaking.InformationCoupling
{
	public class NoInterceptionInformationSource : IInformationSource
	{
		public Vector3 TargetPosition()
		{
			return Vector3.positiveInfinity;
		}

		public float TargetRotationY()
		{
			return 0f;
		}

		public float Weight { get; set; }
	}
}
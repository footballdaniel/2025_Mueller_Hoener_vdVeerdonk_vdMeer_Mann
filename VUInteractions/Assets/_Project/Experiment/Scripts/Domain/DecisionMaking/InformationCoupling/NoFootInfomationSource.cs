using UnityEngine;

namespace Interactions.Domain.DecisionMaking.InformationCoupling
{
	public class NoFootInfomationSource : IInformationSource
	{
		public Vector3 TargetPosition()
		{
			return Vector3.zero;
		}

		public float TargetRotationY()
		{
			return 0f;
		}

		public float Weight { get; set; }
	}
}
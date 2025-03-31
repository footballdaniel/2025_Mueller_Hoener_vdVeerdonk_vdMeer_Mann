using UnityEngine;

namespace Interactions.Domain.DecisionMaking.InformationCoupling
{
	public interface IInformationSource
	{
		Vector3 TargetPosition();
		float TargetRotationY();
		float Weight { get; set; }
	}
}
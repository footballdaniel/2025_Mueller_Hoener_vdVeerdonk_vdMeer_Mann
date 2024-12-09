using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public interface IInformationSource
	{
		Vector3 TargetPosition();
		float TargetRotationY();
		float Weight { get; set; }
	}
}
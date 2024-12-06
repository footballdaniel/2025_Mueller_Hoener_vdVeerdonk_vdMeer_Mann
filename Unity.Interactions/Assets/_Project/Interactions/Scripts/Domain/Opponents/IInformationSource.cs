using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public interface IInformationSource
	{
		Vector3 GetDesiredPosition();
		float Weight { get; set; }
	}
}
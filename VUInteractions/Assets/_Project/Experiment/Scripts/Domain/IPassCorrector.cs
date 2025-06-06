using UnityEngine;

namespace Interactions.Domain
{
	public interface IPassCorrector
	{
		Pass Correct(Pass pass, Vector3 referencePosition);
	}
}
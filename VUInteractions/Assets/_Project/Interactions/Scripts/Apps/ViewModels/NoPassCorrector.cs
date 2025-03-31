using Interactions.Domain;
using UnityEngine;

namespace Interactions.Apps.ViewModels
{
	public class NoPassCorrector : IPassCorrector
	{
		public Pass Correct(Pass pass, Vector3 referencePosition)
		{
			return pass;
		}
	}
}
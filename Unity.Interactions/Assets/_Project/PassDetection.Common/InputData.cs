using System.Collections.Generic;
using UnityEngine;

namespace _Project.PassDetection.Common
{
	public record InputData(List<Vector3> UserDominantFootPositions, List<Vector3> UserNonDominantFootPositions, List<float> Timestamps);
}
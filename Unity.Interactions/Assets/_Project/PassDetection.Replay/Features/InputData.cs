using System.Collections.Generic;
using UnityEngine;

namespace PassDetection.Replay.Features
{
	public record InputData(List<Vector3> UserDominantFootPositions, List<Vector3> UserNonDominantFootPositions, List<float> Timestamps);
}
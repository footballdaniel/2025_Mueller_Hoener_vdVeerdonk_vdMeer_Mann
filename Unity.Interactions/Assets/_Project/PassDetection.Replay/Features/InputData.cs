using System.Collections.Generic;
using Tactive.MachineLearning._Project.MachineLearning;
using UnityEngine;

namespace _Project.PassDetection.Replay.Features
{
	public record InputData(List<Vector3> UserDominantFootPositions, List<Vector3> UserNonDominantFootPositions, List<float> Timestamps);
}
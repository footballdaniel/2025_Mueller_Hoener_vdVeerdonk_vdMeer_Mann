using System.Collections.Generic;
using System.Linq;
using PassDetection.Replay.Features;
using Tactive.MachineLearning._Project.MachineLearning;
using UnityEngine;

namespace _Project.PassDetection.Replay.Features
{
	public class ZeroedPositionDominantFoot : BaseFeature<InputData>
	{
		public override int Size => 3;

		public override List<Feature> Calculate(InputData inputData)
		{
			var dominantPositions = inputData.UserDominantFootPositions;
			var origin = dominantPositions[0];
			var zeroedPositions = dominantPositions
				.Select(p => new Vector3(p.x - origin.x, p.y - origin.y, p.z - origin.z))
				.ToList();

			var xValues = zeroedPositions.Select(pos => pos.x).ToList();
			var yValues = zeroedPositions.Select(pos => pos.y).ToList();
			var zValues = zeroedPositions.Select(pos => pos.z).ToList();

			return new List<Feature>
			{
				new Feature("zeroed_position_dominant_foot_x", xValues),
				new Feature("zeroed_position_dominant_foot_y", yValues),
				new Feature("zeroed_position_dominant_foot_z", zValues)
			};
		}
	}
}
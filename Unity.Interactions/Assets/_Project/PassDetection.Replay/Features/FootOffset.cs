using System.Collections.Generic;
using System.Linq;
using Tactive.MachineLearning._Project.MachineLearning;
using UnityEngine;

namespace _Project.PassDetection.Replay.Features
{
	public class FootOffset : BaseFeature<InputData>
	{
		public override int Size => 3;

		public override List<Feature> Calculate(InputData inputInputData)
		{
			var dominantPositions = inputInputData.UserDominantFootPositions;
			var nonDominantPositions = inputInputData.UserNonDominantFootPositions;

			var offsets = dominantPositions
				.Select((dominant, i) => new Vector3(
					nonDominantPositions[i].x - dominant.x,
					nonDominantPositions[i].y - dominant.y,
					nonDominantPositions[i].z - dominant.z))
				.ToList();

			var xValues = offsets.Select(pos => pos.x).ToList();
			var yValues = offsets.Select(pos => pos.y).ToList();
			var zValues = offsets.Select(pos => pos.z).ToList();

			return new List<Feature>
			{
				new Feature("offset_dominant_foot_to_non_dominant_foot_x", xValues),
				new Feature("offset_dominant_foot_to_non_dominant_foot_y", yValues),
				new Feature("offset_dominant_foot_to_non_dominant_foot_z", zValues)
			};
		}
	}
}
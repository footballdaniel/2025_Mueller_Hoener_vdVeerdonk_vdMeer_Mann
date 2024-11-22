using System.Collections.Generic;
using System.Linq;
using _Project.PassDetection.Replay;
using _Project.PassDetection.Replay.Features;
using UnityEngine;

namespace Src.Domain.Inferences
{
	public class VelocitiesDominantFootCalculator : FeatureCalculator
	{
		public override int Size => 3;

		public override List<Feature> Calculate(InputData inputData)
		{
			var dominantPositions = inputData.UserDominantFootPositions;
			var timestamps = inputData.Timestamps;
			var velocities = new List<Vector3>();

			for (var i = 1; i < dominantPositions.Count; i++)
			{
				var dt = timestamps[i] - timestamps[i - 1];
				if (dt == 0) dt = 1e-6f;

				var dx = (dominantPositions[i].x - dominantPositions[i - 1].x) / dt;
				var dy = (dominantPositions[i].y - dominantPositions[i - 1].y) / dt;
				var dz = (dominantPositions[i].z - dominantPositions[i - 1].z) / dt;
				velocities.Add(new Vector3(dx, dy, dz));
			}

			velocities.Insert(0, Vector3.zero);

			var xValues = velocities.Select(v => v.x).ToList();
			var yValues = velocities.Select(v => v.y).ToList();
			var zValues = velocities.Select(v => v.z).ToList();

			return new List<Feature>
			{
				new("velocities_dominant_foot_x", xValues),
				new("velocities_dominant_foot_y", yValues),
				new("velocities_dominant_foot_z", zValues)
			};
		}
	}
}
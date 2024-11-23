using System.Collections.Generic;
using System.Linq;
using _Project.PassDetection.Replay;
using _Project.PassDetection.Replay.Features;
using Tactive.MachineLearning._Project.MachineLearning;
using UnityEngine;

namespace Src.Domain.Inferences
{
    public class VelocitiesNonDominantFoot : BaseFeature<InputData>
    {
        public override int Size => 3;

        public override List<Feature> Calculate(InputData inputData)
        {
            var nonDominantPositions = inputData.UserNonDominantFootPositions;
            var timestamps = inputData.Timestamps;
            var velocities = new List<Vector3>();

            for (var i = 1; i < nonDominantPositions.Count; i++)
            {
                var dt = timestamps[i] - timestamps[i - 1];
                if (dt == 0) dt = 1e-6f;

                var dx = (nonDominantPositions[i].x - nonDominantPositions[i - 1].x) / dt;
                var dy = (nonDominantPositions[i].y - nonDominantPositions[i - 1].y) / dt;
                var dz = (nonDominantPositions[i].z - nonDominantPositions[i - 1].z) / dt;
                velocities.Add(new Vector3(dx, dy, dz));
            }

            velocities.Insert(0, Vector3.zero);

            var xValues = velocities.Select(v => v.x).ToList();
            var yValues = velocities.Select(v => v.y).ToList();
            var zValues = velocities.Select(v => v.z).ToList();

            return new List<Feature>
            {
                new Feature("velocities_non_dominant_foot_x", xValues),
                new Feature("velocities_non_dominant_foot_y", yValues),
                new Feature("velocities_non_dominant_foot_z", zValues)
            };
        }
    }
}

namespace Src.Domain.Inferences
{
}

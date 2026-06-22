using Interactions.Domain.Opponents;
using UnityEngine;

namespace Interactions.Apps
{
    /// <summary>
    /// Like <see cref="InteractiveBehavior"/> (same spot-picking and pass interception), but shifts
    /// the pressure target laterally by a config-driven offset read when the opponent is spawned.
    /// </summary>
    public class ProactiveBehavior : InteractiveBehavior
    {
        public override void Configure(Opponent opponent, App app)
        {
            base.Configure(opponent, app);

            var config = ExperimentConfig.Load();
            opponent.SetPressureTargetOffset(ComputeTargetOffset(config, app));
        }

        static Vector3 ComputeTargetOffset(ExperimentConfig config, App app)
        {
            // Lateral axis = goal-to-goal direction; "right" points toward the right goal.
            var lateralDirection = app.RightGoal.transform.position - app.LeftGoal.transform.position;
            lateralDirection.y = 0f;
            lateralDirection = lateralDirection.sqrMagnitude > 0.0001f ? lateralDirection.normalized : Vector3.right;

            var movesToRight = Random.Range(0f, 100f) < config.ProbabilityMovementToRightPct;
            var sign = movesToRight ? 1f : -1f;
            var offset = lateralDirection * (config.LateralOffsetMeter * sign);

            Debug.Log($"[Proactive] Target offset {offset} (movesToRight={movesToRight}, magnitude={config.LateralOffsetMeter}m)");
            return offset;
        }
    }
}

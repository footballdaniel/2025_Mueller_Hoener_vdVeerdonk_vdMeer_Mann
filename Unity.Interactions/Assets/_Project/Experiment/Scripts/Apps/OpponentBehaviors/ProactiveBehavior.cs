using System.Collections.Generic;
using System.Linq;
using Interactions.Apps.ViewModels;
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

            var movesToRight = Random.Range(0f, 100f) < config.ProbabilityMovementToRightPct;
            var sign = movesToRight ? 1f : -1f;
            var offsetMeters = Random.Range(config.LateralOffsetMin, config.LateralOffsetMax);

            var positionOffset = LateralDirection(app) * (offsetMeters * sign);
            // Body turns the opposite way to the lateral offset: e.g. 1m right -> degreesPerMeter degrees left.
            var bodyRotationOffset = -sign * offsetMeters * config.BodyOrientationDegreesPerMeter;
            var delay = Random.Range(config.LateralDelayStart, config.LateralDelayEnd);

            opponent.ScheduleLateralOffset(positionOffset, bodyRotationOffset, delay);
            Debug.Log($"[Proactive] offset {positionOffset} (mag {offsetMeters:0.00}m), body {bodyRotationOffset:0.#}deg, after {delay:0.00}s");
        }

        public override IEnumerable<SettingDescriptor> GetSettings(App app)
        {
            // The common opponent settings, plus the proactive-specific ones.
            return base.GetSettings(app).Concat(app.ProactiveSettingsViewModel.GetDescriptors());
        }

        // Lateral axis = goal-to-goal direction; "right" points toward the right goal.
        static Vector3 LateralDirection(App app)
        {
            var direction = app.RightGoal.transform.position - app.LeftGoal.transform.position;
            direction.y = 0f;
            return direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector3.right;
        }
    }
}

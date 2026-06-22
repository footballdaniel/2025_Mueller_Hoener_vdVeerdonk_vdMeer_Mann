using System.Globalization;
using Interactions.Config.Contracts;

namespace Interactions.Apps
{
    /// <summary>
    /// Typed view over the Bronze config for the proactive-interactive mode.
    /// Holds the default values that apply when a key is missing or unparseable.
    /// </summary>
    public class ProactiveInteractionConfig
    {
        public const string LateralOffsetMeterKey = "lateralOffsetMeter";
        public const string ProbabilityMovementToRightPctKey = "ProbabilityMovementToRightPct";

        public const float DefaultLateralOffsetMeter = 1f;
        public const int DefaultProbabilityMovementToRightPct = 50;

        public ProactiveInteractionConfig(IConfig config)
        {
            _config = config;

            if (_config.Get(LateralOffsetMeterKey) == null)
                _config.Set(LateralOffsetMeterKey, DefaultLateralOffsetMeter.ToString(CultureInfo.InvariantCulture));

            if (_config.Get(ProbabilityMovementToRightPctKey) == null)
                _config.Set(ProbabilityMovementToRightPctKey, DefaultProbabilityMovementToRightPct.ToString(CultureInfo.InvariantCulture));
        }

        public float LateralOffsetMeter =>
            float.TryParse(_config.Get(LateralOffsetMeterKey), NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
                ? value
                : DefaultLateralOffsetMeter;

        public int ProbabilityMovementToRightPct =>
            int.TryParse(_config.Get(ProbabilityMovementToRightPctKey), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
                ? value
                : DefaultProbabilityMovementToRightPct;

        readonly IConfig _config;
    }
}

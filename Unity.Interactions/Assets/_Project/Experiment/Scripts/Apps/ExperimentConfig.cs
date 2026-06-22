using System.Globalization;
using System.IO;
using Interactions.Config.Contracts;
using UnityEngine;
using ConfigFile = global::Interactions.Config.Entities.Config;

namespace Interactions.Apps
{
    /// <summary>
    /// Typed view over the Bronze config (persistentDataPath/config.json) for all experiment
    /// settings that are otherwise hard-coded on startup. Holds the default values (which mirror
    /// the previous hard-coded defaults) and seeds any missing key to disk on construction.
    /// </summary>
    public class ExperimentConfig
    {
        public const string FileName = "config.json";

        // Proactive-interactive opponent target offset.
        public const string LateralOffsetMinKey = "lateralOffsetMin";
        public const string LateralOffsetMaxKey = "lateralOffsetMax";
        public const string ProbabilityMovementToRightPctKey = "ProbabilityMovementToRightPct";
        public const string LateralDelayStartKey = "lateralDelayStart";
        public const string LateralDelayEndKey = "lateralDelayEnd";
        public const string BodyOrientationDegreesPerMeterKey = "bodyOrientationDegreesPerMeter";

        // Opponent / experiment settings (mirror the OpponentSettings + pass UI sliders).
        public const string InterPersonalDistanceKey = "interPersonalDistance";
        public const string BodyInformationWeightKey = "bodyInformationWeight";
        public const string FootInformationWeightKey = "footInformationWeight";
        public const string OpponentAccelerationKey = "opponentAcceleration";
        public const string OpponentReactionTimeBodyKey = "opponentReactionTimeBody";
        public const string OpponentReactionTimeFootKey = "opponentReactionTimeFoot";
        public const string DistanceBetweenGoalsKey = "distanceBetweenGoals";
        public const string PassDetectionThresholdKey = "passDetectionThreshold";

        public const float DefaultLateralOffsetMin = 0f;
        public const float DefaultLateralOffsetMax = 3f;
        public const int DefaultProbabilityMovementToRightPct = 50;
        public const float DefaultLateralDelayStart = 1f;
        public const float DefaultLateralDelayEnd = 3f;
        public const float DefaultBodyOrientationDegreesPerMeter = 30f;
        public const float DefaultInterPersonalDistance = 4f;
        public const float DefaultBodyInformationWeight = 0.5f;
        public const float DefaultFootInformationWeight = 0.33f;
        public const float DefaultOpponentAcceleration = 10f;
        public const float DefaultOpponentReactionTimeBody = 1f;
        public const float DefaultOpponentReactionTimeFoot = 0.4f;
        public const float DefaultDistanceBetweenGoals = 2.5f;
        public const float DefaultPassDetectionThreshold = 0.9f;

        /// <summary>Loads (and seeds) the config from persistentDataPath/config.json.</summary>
        public static ExperimentConfig Load()
        {
            var path = Path.Combine(Application.persistentDataPath, FileName);
            return new ExperimentConfig(new ConfigFile(path));
        }

        public ExperimentConfig(IConfig config)
        {
            _config = config;
            SeedDefaults();
        }

        // Getters fall back to defaults; setters write through to config.json immediately.
        public float LateralOffsetMin { get => GetFloat(LateralOffsetMinKey, DefaultLateralOffsetMin); set => SetFloat(LateralOffsetMinKey, value); }
        public float LateralOffsetMax { get => GetFloat(LateralOffsetMaxKey, DefaultLateralOffsetMax); set => SetFloat(LateralOffsetMaxKey, value); }
        public int ProbabilityMovementToRightPct { get => GetInt(ProbabilityMovementToRightPctKey, DefaultProbabilityMovementToRightPct); set => SetInt(ProbabilityMovementToRightPctKey, value); }
        public float LateralDelayStart { get => GetFloat(LateralDelayStartKey, DefaultLateralDelayStart); set => SetFloat(LateralDelayStartKey, value); }
        public float LateralDelayEnd { get => GetFloat(LateralDelayEndKey, DefaultLateralDelayEnd); set => SetFloat(LateralDelayEndKey, value); }
        public float BodyOrientationDegreesPerMeter { get => GetFloat(BodyOrientationDegreesPerMeterKey, DefaultBodyOrientationDegreesPerMeter); set => SetFloat(BodyOrientationDegreesPerMeterKey, value); }
        public float InterPersonalDistance { get => GetFloat(InterPersonalDistanceKey, DefaultInterPersonalDistance); set => SetFloat(InterPersonalDistanceKey, value); }
        public float BodyInformationWeight { get => GetFloat(BodyInformationWeightKey, DefaultBodyInformationWeight); set => SetFloat(BodyInformationWeightKey, value); }
        public float FootInformationWeight { get => GetFloat(FootInformationWeightKey, DefaultFootInformationWeight); set => SetFloat(FootInformationWeightKey, value); }
        public float OpponentAcceleration { get => GetFloat(OpponentAccelerationKey, DefaultOpponentAcceleration); set => SetFloat(OpponentAccelerationKey, value); }
        public float OpponentReactionTimeBody { get => GetFloat(OpponentReactionTimeBodyKey, DefaultOpponentReactionTimeBody); set => SetFloat(OpponentReactionTimeBodyKey, value); }
        public float OpponentReactionTimeFoot { get => GetFloat(OpponentReactionTimeFootKey, DefaultOpponentReactionTimeFoot); set => SetFloat(OpponentReactionTimeFootKey, value); }
        public float DistanceBetweenGoals { get => GetFloat(DistanceBetweenGoalsKey, DefaultDistanceBetweenGoals); set => SetFloat(DistanceBetweenGoalsKey, value); }
        public float PassDetectionThreshold { get => GetFloat(PassDetectionThresholdKey, DefaultPassDetectionThreshold); set => SetFloat(PassDetectionThresholdKey, value); }

        void SeedDefaults()
        {
            Seed(LateralOffsetMinKey, DefaultLateralOffsetMin);
            Seed(LateralOffsetMaxKey, DefaultLateralOffsetMax);
            Seed(ProbabilityMovementToRightPctKey, DefaultProbabilityMovementToRightPct);
            Seed(LateralDelayStartKey, DefaultLateralDelayStart);
            Seed(LateralDelayEndKey, DefaultLateralDelayEnd);
            Seed(BodyOrientationDegreesPerMeterKey, DefaultBodyOrientationDegreesPerMeter);
            Seed(InterPersonalDistanceKey, DefaultInterPersonalDistance);
            Seed(BodyInformationWeightKey, DefaultBodyInformationWeight);
            Seed(FootInformationWeightKey, DefaultFootInformationWeight);
            Seed(OpponentAccelerationKey, DefaultOpponentAcceleration);
            Seed(OpponentReactionTimeBodyKey, DefaultOpponentReactionTimeBody);
            Seed(OpponentReactionTimeFootKey, DefaultOpponentReactionTimeFoot);
            Seed(DistanceBetweenGoalsKey, DefaultDistanceBetweenGoals);
            Seed(PassDetectionThresholdKey, DefaultPassDetectionThreshold);
        }

        void Seed(string key, float value)
        {
            if (_config.Get(key) == null)
                _config.Set(key, value.ToString(CultureInfo.InvariantCulture));
        }

        void Seed(string key, int value)
        {
            if (_config.Get(key) == null)
                _config.Set(key, value.ToString(CultureInfo.InvariantCulture));
        }

        float GetFloat(string key, float fallback) =>
            float.TryParse(_config.Get(key), NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : fallback;

        int GetInt(string key, int fallback) =>
            int.TryParse(_config.Get(key), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : fallback;

        void SetFloat(string key, float value) => _config.Set(key, value.ToString(CultureInfo.InvariantCulture));

        void SetInt(string key, int value) => _config.Set(key, value.ToString(CultureInfo.InvariantCulture));

        readonly IConfig _config;
    }
}

using System.Collections.Generic;

namespace Interactions.Apps.ViewModels
{
	public class ProactiveSettingsViewModel
	{
		public ProactiveSettingsViewModel(App app)
		{
			_app = app;
		}

		public float LateralOffsetMin { get => _app.Config.LateralOffsetMin; set => _app.Config.LateralOffsetMin = value; }
		public float LateralOffsetMax { get => _app.Config.LateralOffsetMax; set => _app.Config.LateralOffsetMax = value; }
		public float ProbabilityMovingToDominantFootSidePct { get => _app.Config.ProbabilityMovingToDominantFootSidePct; set => _app.Config.ProbabilityMovingToDominantFootSidePct = (int)value; }
		public float LateralDelayStart { get => _app.Config.LateralDelayStart; set => _app.Config.LateralDelayStart = value; }
		public float LateralDelayEnd { get => _app.Config.LateralDelayEnd; set => _app.Config.LateralDelayEnd = value; }
		public float BodyOrientationDegreesPerMeter { get => _app.Config.BodyOrientationDegreesPerMeter; set => _app.Config.BodyOrientationDegreesPerMeter = value; }

		public IEnumerable<SettingDescriptor> GetDescriptors()
		{
			return new[]
			{
				new SettingDescriptor("Lateral Offset Min (m)", LateralOffsetMin, 0f, 3f, value => LateralOffsetMin = value),
				new SettingDescriptor("Lateral Offset Max (m)", LateralOffsetMax, 0f, 3f, value => LateralOffsetMax = value),
				new SettingDescriptor("Move to Dominant Foot Side (%)", ProbabilityMovingToDominantFootSidePct, 0f, 100f, value => ProbabilityMovingToDominantFootSidePct = value),
				new SettingDescriptor("Delay Start (s)", LateralDelayStart, 0f, 10f, value => LateralDelayStart = value),
				new SettingDescriptor("Delay End (s)", LateralDelayEnd, 0f, 10f, value => LateralDelayEnd = value),
				new SettingDescriptor("Body Orientation (deg/m)", BodyOrientationDegreesPerMeter, 0f, 90f, value => BodyOrientationDegreesPerMeter = value),
			};
		}

		readonly App _app;
	}
}

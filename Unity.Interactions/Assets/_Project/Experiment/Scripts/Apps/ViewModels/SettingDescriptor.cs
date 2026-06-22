using System;

namespace Interactions.Apps.ViewModels
{
	public class SettingDescriptor
	{
		public SettingDescriptor(string label, float value, float min, float max, Action<float> onChanged)
		{
			Label = label;
			Value = value;
			Min = min;
			Max = max;
			OnChanged = onChanged;
		}

		public string Label { get; }
		public float Value { get; }
		public float Min { get; }
		public float Max { get; }
		public Action<float> OnChanged { get; }
	}
}

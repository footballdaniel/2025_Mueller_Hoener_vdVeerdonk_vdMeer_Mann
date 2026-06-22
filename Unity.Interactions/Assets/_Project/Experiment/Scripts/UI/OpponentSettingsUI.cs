using System.Collections.Generic;
using Interactions.Apps.ViewModels;
using UnityEngine;

namespace Interactions.UI
{
	/// <summary>
	/// Settings panel generated dynamically from a list of <see cref="SettingDescriptor"/>, so each
	/// condition shows exactly the sliders that apply to its opponent (e.g. proactive adds its own).
	/// </summary>
	public class OpponentSettingsUI : UIScreen
	{
		[SerializeField] SettingSlider _sliderPrefab;
		[SerializeField] Transform _container;

		public void Bind(IEnumerable<SettingDescriptor> settings)
		{
			Clear();

			foreach (var setting in settings)
			{
				var descriptor = setting;
				var slider = Instantiate(_sliderPrefab, _container);
				slider.Slider.onValueChanged.AddListener(value => descriptor.OnChanged(value));
				slider.Bind(descriptor.Label, descriptor.Value, descriptor.Min, descriptor.Max);
			}
		}

		void Clear()
		{
			foreach (Transform child in _container)
				Destroy(child.gameObject);
		}

		void OnDisable()
		{
			Clear();
		}
	}
}

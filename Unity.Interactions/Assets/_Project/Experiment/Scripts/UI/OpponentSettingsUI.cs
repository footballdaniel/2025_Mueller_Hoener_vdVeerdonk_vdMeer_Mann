using System.Collections.Generic;
using Interactions.Apps.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace Interactions.UI
{
	public class OpponentSettingsUI : UIScreen
	{
		[SerializeField] SettingSlider _sliderPrefab;
		[SerializeField] Transform _container;

		Transform Container => _container != null ? _container : transform;

		public void Bind(IEnumerable<SettingDescriptor> settings)
		{
			EnsureContainerFitsContent();
			Clear();

			foreach (var setting in settings)
			{
				var descriptor = setting;
				var slider = Instantiate(_sliderPrefab, Container);
				slider.Slider.onValueChanged.AddListener(value => descriptor.OnChanged(value));
				slider.Bind(descriptor.Label, descriptor.Value, descriptor.Min, descriptor.Max);
			}
		}

		void EnsureContainerFitsContent()
		{
			var fitter = Container.GetComponent<ContentSizeFitter>();
			if (fitter == null)
				fitter = Container.gameObject.AddComponent<ContentSizeFitter>();
			fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		}

		void Clear()
		{
			foreach (Transform child in Container)
				Destroy(child.gameObject);
		}

		void OnDisable()
		{
			Clear();
		}
	}
}

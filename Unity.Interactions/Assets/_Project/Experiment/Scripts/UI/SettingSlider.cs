using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interactions.UI
{
	public class SettingSlider : MonoBehaviour
	{

		[SerializeField] TMP_Text _title;
		[SerializeField] TMP_Text _min;
		[SerializeField] TMP_Text _max;
		[field: SerializeReference] public Slider Slider { get; private set; }

		public void Bind(string title, float value, float minValue, float maxValue)
		{
			_title.text = title;
			_min.text = minValue.ToString();
			_max.text = maxValue.ToString();
			Slider.minValue = minValue;
			Slider.maxValue = maxValue;
			Slider.value = value;
		}
	}
}
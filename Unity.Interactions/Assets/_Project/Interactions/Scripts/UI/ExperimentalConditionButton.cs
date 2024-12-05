using Interactions.Scripts.Application;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Interactions.Scripts.UI
{
	public class ExperimentalConditionButton : Button
	{
		[SerializeField] TMP_Text _text;
		
		void Awake()
		{
			_text = GetComponentInChildren<TMP_Text>();
		}

		public void Set(ExperimentalCondition condition)
		{
			_text.SetText(condition.ToString());
		}
	}
}
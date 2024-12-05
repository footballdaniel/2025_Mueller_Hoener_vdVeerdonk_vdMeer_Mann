using Interactions.Scripts.Application;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Interactions.Scripts.UI
{
	public class ExperimentalConditionButton : Button
	{
		[SerializeField] TMP_Text _text;

		public void Set(ExperimentalCondition condition)
		{
			_text.SetText(condition.ToString());
		}
	}
}
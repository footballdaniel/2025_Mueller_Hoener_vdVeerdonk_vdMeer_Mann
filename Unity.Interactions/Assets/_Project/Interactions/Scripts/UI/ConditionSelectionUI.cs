using System;
using Interactions.Scripts.Application;
using UnityEngine;

namespace Interactions.Scripts.UI
{
	public class ConditionSelectionUI : UIScreen
	{
		[SerializeField] ExperimentalConditionButton _conditionButtonPrefab;
		[SerializeField] Transform _conditionButtonContainer;


		public void Bind(ConditionSelection conditionSelection)
		{
			foreach (var condition in Enum.GetValues(typeof(ExperimentalCondition)))
			{
				var conditionButton = Instantiate(_conditionButtonPrefab, _conditionButtonContainer);
				conditionButton.Set((ExperimentalCondition)condition);
				conditionButton.Button.onClick.AddListener(() => conditionSelection.ConditionSelected((ExperimentalCondition)condition));
			}
		}
	}
}
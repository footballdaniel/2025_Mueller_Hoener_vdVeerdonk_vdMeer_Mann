using System;
using _Project.Interactions.Scripts.UI;
using Interactions.Scripts.Application;
using UnityEngine;

public class ConditionSelectionUI : MonoBehaviour
{
	[SerializeField] ExperimentalConditionButton _conditionButtonPrefab;
	[SerializeField] Transform _conditionButtonContainer;
	
	
	public void Bind(ConditionSelection conditionSelection)
	{
		foreach (var condition in Enum.GetValues(typeof(ExperimentalCondition)))
		{
			var button = Instantiate(_conditionButtonPrefab, _conditionButtonContainer);
			button.Set((ExperimentalCondition) condition);
			button.onClick.AddListener(() => conditionSelection.ConditionSelected((ExperimentalCondition) condition));
		}

	}
}

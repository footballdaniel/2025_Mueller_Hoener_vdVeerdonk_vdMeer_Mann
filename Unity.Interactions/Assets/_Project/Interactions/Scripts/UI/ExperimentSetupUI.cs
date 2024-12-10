using System;
using Interactions.Apps;
using Interactions.Apps.States;
using Interactions.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace Interactions.UI
{
	public class ExperimentSetupUI : UIScreen
	{
		[SerializeField] ExperimentalConditionButton _conditionButtonPrefab;
		[SerializeField] Transform _conditionButtonContainer;
		[SerializeField] Button _leftSideDominantButton;
		[SerializeField] Button _rightSideDominantButton;


		void OnDisable()
		{
			_leftSideDominantButton.onClick.RemoveAllListeners();
			_rightSideDominantButton.onClick.RemoveAllListeners();
		}

		public void Bind(StartExperiment context)
		{
			foreach (var condition in Enum.GetValues(typeof(ExperimentalCondition)))
			{
				var conditionButton = Instantiate(_conditionButtonPrefab, _conditionButtonContainer);
				conditionButton.Set((ExperimentalCondition)condition);
				conditionButton.Button.onClick.AddListener(() => context.ConditionSelected((ExperimentalCondition)condition));
			}
			
			_leftSideDominantButton.onClick.AddListener(() => context.DominantFootSelected(Side.LEFT));
			_rightSideDominantButton.onClick.AddListener(() => context.DominantFootSelected(Side.RIGHT));
		}
	}
}
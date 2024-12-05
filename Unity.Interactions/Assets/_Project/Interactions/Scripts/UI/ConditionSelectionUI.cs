using Interactions.Scripts.Application;
using UnityEngine;

public class ConditionSelectionUI : MonoBehaviour
{
	public void Bind(ConditionSelection conditionSelection)
	{
		foreach (var condition in ExperimentalCondition.All)
		{
			var button = Instantiate(Resources.Load<GameObject>("ConditionButton"), transform);
			button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = condition.Name;
			button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => conditionSelection.ConditionSelected(condition));
		}
	}
}

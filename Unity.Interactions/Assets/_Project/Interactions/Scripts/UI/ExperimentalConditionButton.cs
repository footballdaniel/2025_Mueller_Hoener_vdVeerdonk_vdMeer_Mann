using Interactions.Scripts.Application;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interactions.Scripts.UI
{
	public class ExperimentalConditionButton : MonoBehaviour
	{
		[field:SerializeReference] public Button Button { get; private set; }
		[SerializeField] TMP_Text _text;
		
		public void Set(ExperimentalCondition condition)
		{
			_text.SetText(condition.ToString());
		}
	}
}
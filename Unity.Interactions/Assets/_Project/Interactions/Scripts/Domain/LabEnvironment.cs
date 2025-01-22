using UnityEngine;

namespace Interactions.Domain
{
	public class LabEnvironment : MonoBehaviour
	{
		public void Hide()
		{
			gameObject.SetActive(false);
		}

		public void Show()
		{
			gameObject.SetActive(true);
		}
	}
}
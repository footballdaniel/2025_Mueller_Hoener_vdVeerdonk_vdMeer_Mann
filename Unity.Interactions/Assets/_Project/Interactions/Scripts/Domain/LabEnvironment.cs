using UnityEngine;

namespace Interactions.Domain
{
	public class LabEnvironment : MonoBehaviour
	{
		public void Hide()
		{
			gameObject.SetActive(false);
			IsVisible = false;
		}

		public void Show()
		{
			gameObject.SetActive(true);
			IsVisible = true;
		}

		public bool IsVisible { get; private set; }
	}
}
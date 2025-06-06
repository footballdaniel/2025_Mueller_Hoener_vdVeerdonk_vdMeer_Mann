using UnityEngine;

namespace Interactions.UI
{
	public abstract class UIScreen : MonoBehaviour
	{
		void Awake()
		{
			Hide();
		}

		public virtual void Hide()
		{
			gameObject.SetActive(false);
		}

		public virtual void Show()
		{
			gameObject.SetActive(true);
		}
	}
}
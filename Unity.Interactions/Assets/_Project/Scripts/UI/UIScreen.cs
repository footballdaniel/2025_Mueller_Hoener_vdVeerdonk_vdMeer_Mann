using UnityEngine;

namespace UI
{
	public abstract class UIScreen : MonoBehaviour
	{
		void Awake()
		{
			Hide();
		}

		public virtual void Show()
		{
			gameObject.SetActive(true);
		}
		
		public virtual void Hide()
		{
			gameObject.SetActive(false);
		}
		
	}
}
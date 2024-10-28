using App;
using TMPro;
using UnityEngine;

namespace UI
{
	public class XRStatusUI : UIScreen
	{
		[SerializeField] TMP_Text _errorText;

		public void Bind(XRStatusViewModel xrStatusViewModel)
		{
			xrStatusViewModel.XRStartupErrorOccurred += OnXRStartupErrorOccurred;
		}

		void OnXRStartupErrorOccurred()
		{
			_errorText.gameObject.SetActive(true);
		}
	}
}
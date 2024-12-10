using Interactions.Apps.ViewModels;
using TMPro;
using UnityEngine;

namespace Interactions.UI
{
	public class XRStatusUI : UIScreen
	{	
		[SerializeField] TMP_Text _errorText;

		public void Bind(XRStatusViewModel viewModel)
		{
			if (viewModel.HasErrors())
				_errorText.gameObject.SetActive(true);
			else
				viewModel.Startup();
		}
	}
}
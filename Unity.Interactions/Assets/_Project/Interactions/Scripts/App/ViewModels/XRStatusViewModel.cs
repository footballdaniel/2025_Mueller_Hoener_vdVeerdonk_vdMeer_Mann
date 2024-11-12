using System;

namespace _Project.Interactions.Scripts.App.ViewModels
{
	public class XRStatusViewModel
	{

		public XRStatusViewModel()
		{
			XRStatus.XRStartupError += OnXRStartupError;
		}

		public event Action XRStartupErrorOccurred;

		void OnXRStartupError()
		{
			XRStartupErrorOccurred?.Invoke();
		}
	}
}
using System;

namespace App
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
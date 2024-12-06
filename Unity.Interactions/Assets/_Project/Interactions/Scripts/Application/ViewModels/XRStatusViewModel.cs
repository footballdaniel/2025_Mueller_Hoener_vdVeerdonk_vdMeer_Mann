using System;

namespace Interactions.Application.ViewModels
{
	public class XRStatusViewModel
	{

		public XRStatusViewModel(App app)
		{
			_app = app;
		}

		public event Action XRStartupErrorOccurred;

		public void CheckForErrors()
		{
			if (XRStatusChecker.HasXRErrors())
				XRStartupErrorOccurred?.Invoke();
			else
				_app.Transitions.SelectCondition.Execute();
		}

		readonly App _app;
	}
}
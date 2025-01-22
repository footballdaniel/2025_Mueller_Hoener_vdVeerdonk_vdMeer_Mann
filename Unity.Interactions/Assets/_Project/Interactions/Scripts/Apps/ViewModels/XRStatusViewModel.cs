namespace Interactions.Apps.ViewModels
{
	public class XRStatusViewModel
	{

		public XRStatusViewModel(App app)
		{
			_app = app;
		}


		public void Startup()
		{
			_app.Transitions.StartExperiment.Execute();
		}

		public bool HasErrors()
		{
			return XRStatusChecker.HasXRErrors();
		}

		readonly App _app;
	}
}
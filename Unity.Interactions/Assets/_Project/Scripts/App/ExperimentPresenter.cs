using System.Collections.Generic;

namespace _Project.Scripts.App
{
	public class ExperimentPresenter
	{
		public Observable<bool> CanStartNextTrial { get; private set; } = new Observable<bool>(false);
		public List<WebCamConfiguration> AvailableWebCams { get; private set; }
		
		public ExperimentPresenter(AvailableWebCams availableWebCams)
		{
			
		}

		public void NextTrial()
		{
			
			CanStartNextTrial.Value = false;
		}
		
	}
}
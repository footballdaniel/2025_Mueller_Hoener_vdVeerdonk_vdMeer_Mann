using System.Collections.Generic;

namespace _Project.Scripts.App
{
	public class ExperimentPresenter
	{
		public Observable<bool> CanStartNextTrial { get; private set; } = new Observable<bool>(false);
		public List<WebCamConfiguration> AvailableWebCams { get; private set; }
		
		public ExperimentPresenter(AvailableWebCams availableWebCams)
		{
			Events.TrialEnded += () => CanStartNextTrial.Value = true;
		}

		public void NextTrial()
		{
			Events.NextTrialRequested?.Invoke();
			CanStartNextTrial.Value = false;
		}
		
	}
}
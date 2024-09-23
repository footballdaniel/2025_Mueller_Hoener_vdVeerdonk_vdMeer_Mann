using System.Collections.Generic;

namespace _Project.Scripts.App
{
	public class ExperimentPresenter
	{

		public ExperimentPresenter(AvailableWebCams availableWebCams, Transitions transitions)
		{
			_transitions = transitions;
		}

		public List<WebCamConfiguration> AvailableWebCams { get; private set; }
		public Observable<bool> CanStartNextTrial { get; set; } = new(false);

		public void NextTrial()
		{
			_transitions.StartTrial.Execute();
		}

		readonly Transitions _transitions;
	}
}
namespace _Project.Scripts.App
{
	public class ExperimentPresenter
	{
		public ExperimentPresenter(UI ui)
		{
			ui.Set(this);
			
			Events.TrialEnded += ui.EnableNextTrial;
		}

		public void NextTrial()
		{
			Events.NextTrialRequested?.Invoke();
		}
		
	}
}
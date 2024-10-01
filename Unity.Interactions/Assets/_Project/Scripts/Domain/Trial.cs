namespace Domain
{
	public class Trial
	{
		public Trial(float startTime)
		{
			StartTime = startTime;
		}

		public void Tick(float deltaTime)
		{
			Duration += deltaTime;
		}

		public float StartTime { get; }
		public float Duration { get; private set; }
	}
}
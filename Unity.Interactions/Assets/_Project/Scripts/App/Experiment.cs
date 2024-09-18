using UnityEngine;

namespace _Project.Scripts.App
{
	public class Experiment
	{
		public Trial NextTrial()
		{
			return new Trial(Time.timeSinceLevelLoad);
		}
	
	}
}
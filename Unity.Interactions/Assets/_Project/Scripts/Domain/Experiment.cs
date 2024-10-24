using Domain;
using UnityEngine;

namespace App
{
	public class Experiment
	{
		public Trial NextTrial()
		{
			return new Trial(Time.timeSinceLevelLoad);
		}
	}
}
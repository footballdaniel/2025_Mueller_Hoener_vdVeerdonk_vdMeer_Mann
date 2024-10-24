using Domain;
using Domain.VideoRecorder;
using UnityEngine;

namespace App
{
	public class Experiment
	{
		public IWebcamRecorder WebcamRecorder { get; set; }
		public Opponent Opponent { get; set; }
		public Ball Ball { get; set; }
		
		public Trial CurrentTrial { get; private set; }
		
		public void NextTrial()
		{
			CurrentTrial =  new Trial(Time.timeSinceLevelLoad);
		}
	}
}
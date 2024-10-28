using Domain;
using Domain.VideoRecorder;
using UnityEngine;

namespace App
{
	public class Experiment
	{
		int _currentTrialIndex;
		public IWebcamRecorder WebcamRecorder { get; set; }
		public Opponent Opponent { get; set; }
		public Ball Ball { get; set; }
		public Trial CurrentTrial { get; private set; }
		
		public void NextTrial()
		{
			_currentTrialIndex++;
			CurrentTrial =  new Trial(_currentTrialIndex);
		}
	}
}
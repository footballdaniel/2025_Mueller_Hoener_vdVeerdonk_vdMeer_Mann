using UnityEngine;

namespace App.States
{
	internal class StartRecordingVideo : State
	{
		public StartRecordingVideo(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_app.Session.WebcamRecorder.StartRecording();
		}

		public override void Tick()
		{
			if (_app.Session.WebcamRecorder.IsRecording)
			{
				Debug.Log("Recording");
				_app.Transitions.StartTrialWithVideoRecording.Execute();
			}
		}
	}
}
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
			var recorder = _app.WebcamRecorderFactory.Create();
			// recorder.
			// _app.WebcamRecorder.StartRecording();
		}

		public override void Tick()
		{
			if (_app.WebcamRecorderPrefab.IsRecording)
			{
				Debug.Log("Recording");
				_app.Transitions.StartTrialWithRecording.Execute();
			}
		}
	}
}
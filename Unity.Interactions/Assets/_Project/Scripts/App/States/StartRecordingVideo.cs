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
			_app.WebcamRecorder = new WebcamRecorderFactory(_app.WebCamConfiguration).Create();
			_app.WebcamRecorder.StartRecording();
		}

		public override void Tick()
		{
			if (_app.WebcamRecorder.IsRecording)
			{
				Debug.Log("Recording");
				_app.Transitions.StartTrialWithRecording.Execute();
			}
		}
	}
}
using App.States;

namespace App
{
	internal class ExportVideoState : State
	{
		public ExportVideoState(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_app.WebcamRecorder.StopRecording();
			_app.WebcamRecorder.Export();
		}

		public override void Tick()
		{
			if (_app.WebcamRecorder.IsExportComplete)
				_app.Transitions.FinishExport.Execute();
		}
	}
}
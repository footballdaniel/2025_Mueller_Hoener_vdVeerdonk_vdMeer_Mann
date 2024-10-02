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
			_app.WebcamRecorderPrefab.StopRecording();
			_app.WebcamRecorderPrefab.Export();
		}

		public override void Tick()
		{
			if (_app.WebcamRecorderPrefab.IsExportComplete)
				_app.Transitions.FinishExport.Execute();
		}
	}
}
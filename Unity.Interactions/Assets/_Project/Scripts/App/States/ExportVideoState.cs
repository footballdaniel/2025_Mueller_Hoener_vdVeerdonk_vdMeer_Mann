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
			ProgressIndicator.Instance.Display("Exporting...", "Frame export", 100);
			
			_app.TrialState.WebcamRecorder.StopRecording();
			_app.TrialState.WebcamRecorder.Export();
		}

		public override void Tick()
		{
			if (_app.TrialState.WebcamRecorder.IsExportComplete)
				_app.Transitions.FinishExport.Execute();
		}
	}
}
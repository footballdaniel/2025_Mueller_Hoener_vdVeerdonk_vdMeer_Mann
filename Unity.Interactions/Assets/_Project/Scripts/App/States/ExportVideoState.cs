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

			_app.Experiment.WebcamRecorder.StopRecording();
			_app.Experiment.WebcamRecorder.Export();
		}

		public override void Tick()
		{
			if (!_app.Experiment.WebcamRecorder.IsExportComplete)
				return;
			
			
			switch (_app.ExperimentalCondition)
			{
				case ExperimentalCondition.Laboratory:
					_app.Transitions.EndLabTrialAfterExporting.Execute();
					break;
				case ExperimentalCondition.InSitu:
					_app.Transitions.EndInSituTrialAfterExporting.Execute();
					break;
			}
		}
	}
}
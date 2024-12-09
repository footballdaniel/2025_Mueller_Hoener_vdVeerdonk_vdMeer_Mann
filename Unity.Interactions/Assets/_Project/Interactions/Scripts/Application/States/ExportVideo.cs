using Interactions.Domain.VideoRecorder;

namespace Interactions.Application.States
{
	internal class ExportVideo : State
	{
		public ExportVideo(App app) : base(app)
		{
		}

		public override void Enter()
		{
			ProgressIndicator.Instance.Display("Exporting...", "Frame export", 100);

			_app.Experiment.WebcamRecorder.StopRecording();
			_app.Experiment.WebcamRecorder.Export(_app.Experiment.CurrentTrial.TrialNumber);
		}

		public override void Tick()
		{
			if (!_app.Experiment.WebcamRecorder.IsExportComplete)
				return;
			
			_app.Transitions.WaitForNextTrial.Execute();
		}
	}
}
using App.States;
using Domain.VideoRecorder;

namespace App
{
	internal class SelectWebcam : State
	{
		public SelectWebcam(App app) : base(app)
		{
		}

		public override void Enter()
		{
			var availableWebCams = new WebcamRepository();
			var presenter = new WebcamSelectionPresenter(availableWebCams, _app);
			_app.UI.WebcamSelectionUI.Set(presenter);
		}
	}
}
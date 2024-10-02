using App.States;

namespace App
{
	internal class SelectWebcam : State
	{
		public SelectWebcam(App app) : base(app)
		{
		}

		public override void Enter()
		{
			var presenter = new WebcamSelectionPresenter(_app.WebCamRecorders, _app);
			_app.UI.WebcamSelectionUI.Set(presenter);
			_app.UI.WebcamSelectionUI.Show();
		}

		public override void Exit()
		{
			_app.UI.WebcamSelectionUI.Hide();
		}
	}
}
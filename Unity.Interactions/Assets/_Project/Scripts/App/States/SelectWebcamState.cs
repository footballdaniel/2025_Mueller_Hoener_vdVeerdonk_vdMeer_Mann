using App.States;

namespace App
{
	internal class SelectWebcamState : State
	{
		public SelectWebcamState(App app) : base(app)
		{
		}

		public override void Enter()
		{
			var presenter = new WebcamSelectionViewModel(_app.WebCamRecorders, _app);
			_app.UI.WebcamSelectionUI.Set(presenter);
			_app.UI.WebcamSelectionUI.Show();
		}

		public override void Exit()
		{
			_app.UI.WebcamSelectionUI.Hide();
		}
	}
}
namespace Interactions.Application.States
{
	internal class SelectWebcam : State
	{
		public SelectWebcam(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_app.UI.WebcamSelectionUI.Bind(_app.WebcamSelectionViewModel);
			_app.UI.WebcamSelectionUI.Show();
		}

		public override void Exit()
		{
			_app.UI.WebcamSelectionUI.Hide();
		}
	}
}
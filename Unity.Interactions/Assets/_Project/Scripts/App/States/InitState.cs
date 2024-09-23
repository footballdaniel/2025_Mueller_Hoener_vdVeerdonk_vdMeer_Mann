namespace _Project.Scripts.App.States
{
	internal class InitState : State
	{

		public InitState(App app) : base(app)
		{
		}


		public override void Enter()
		{
		}

		public override void Tick()
		{
			if (_app.RecordVideo)
				_app.Transitions.RecordVideo.Execute();
			else
				_app.Transitions.StartTrial.Execute();
		}
		
		
	}
}
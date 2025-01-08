using UnityEngine;

namespace Interactions.Apps.States
{
	internal class WaitForNextTrial : State
	{
		float _updateTimer;

		public WaitForNextTrial(App app) : base(app)
		{
		}

		public override void Enter()
		{
			_app.UI.ExperimentUI.Bind(_app.ExperimentViewModel);
			_app.UI.ExperimentUI.Show();
		}

		public override void Tick()
		{
			var deltaTime = 1f / _app.Experiment.FrameRateHz;
			_updateTimer += Time.deltaTime;
			var epsilon = 0.0001f;

			if (_updateTimer >= deltaTime - epsilon)
			{ 
				_app.Experiment.WebcamRecorder.Tick();
				_updateTimer -= deltaTime;
			}
		}
	}
}
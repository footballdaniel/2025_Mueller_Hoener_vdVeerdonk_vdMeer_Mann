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
			_app.UI._experimentUI.Bind(_app.ExperimentViewModel);
			_app.UI._experimentUI.Show();
		}

		public override void Tick()
		{
			var deltaTime = 1f / _app.RecordingFrameRateHz;
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
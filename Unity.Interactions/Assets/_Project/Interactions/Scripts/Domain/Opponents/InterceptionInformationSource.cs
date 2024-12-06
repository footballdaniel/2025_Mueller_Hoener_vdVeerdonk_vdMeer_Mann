using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class InterceptionInformationSource : IInformationSource
	{

		public InterceptionInformationSource(Transform self, Ball ball)
		{
			_self = self;
			_ball = ball;
		}

		public Vector3 GetDesiredPosition()
		{
			var ballPosition = _ball.transform.position;
			var ballDirection = _ball.Velocity.normalized;
			return ballPosition + ballDirection * (_self.position.x - ballPosition.x);
		}

		public float Weight { get; set; }
		readonly Ball _ball;
		Transform _self;
	}
}
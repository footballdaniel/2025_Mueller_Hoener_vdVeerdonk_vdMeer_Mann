using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class InterceptionInformationSource : IInformationSource
	{

		public InterceptionInformationSource(Opponent opponent, Ball ball)
		{
			_opponent = opponent;
			_ball = ball;
		}

		public Vector3 GetDesiredPosition()
		{
			var ballPosition = _ball.transform.position;
			var ballDirection = _ball.Velocity.normalized;
			return ballPosition + ballDirection * (_opponent.Position.x - ballPosition.x);
		}

		public float Weight { get; set; }
		readonly Ball _ball;
		Transform _self;
		readonly Opponent _opponent;
	}
	
	public class NoInterceptionInformationSource : IInformationSource
	{
		public Vector3 GetDesiredPosition()
		{
			return Vector3.positiveInfinity;
		}

		public float Weight { get; set; }
	}
}
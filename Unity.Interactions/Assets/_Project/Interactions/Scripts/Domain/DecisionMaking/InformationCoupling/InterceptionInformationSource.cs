using Interactions.Domain.Opponents;
using UnityEngine;

namespace Interactions.Domain.DecisionMaking.InformationCoupling
{
	public class InterceptionInformationSource : IInformationSource
	{
		readonly Ball _ball;
		readonly Opponent _opponent;

		public InterceptionInformationSource(Opponent opponent, Ball ball)
		{
			_opponent = opponent;
			_ball = ball;
		}

		public Vector3 TargetPosition()
		{
			var ballPosition = _ball.transform.position;
			return ballPosition;
		}

		public float TargetRotationY()
		{
			var ballPosition = _ball.transform.position;
			var dir = (ballPosition - _opponent.Position).normalized;
			return Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
		}

		public float Weight { get; set; }
	}

}
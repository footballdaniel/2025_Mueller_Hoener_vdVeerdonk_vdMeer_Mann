using UnityEngine;

namespace Interactions.Domain.Opponents
{
	[ExecuteInEditMode]
	internal class Legs : MonoBehaviour
	{
		[SerializeField] Ball _ball;
		[SerializeField] Animator _animator;
		[SerializeField] Transform _foot;
		[SerializeField] float _startKickDistance = 1.5f;
		void Update()
		{
			if (!_ball)
				return;
			
			var distanceBallToFoot = Vector3.Distance(_ball.transform.position, _foot.position);
			var distancePlayerToBall = Vector3.Distance(transform.position, _ball.transform.position);

			// lerp between start kick distance (0) and distance (1)
			var kickProgressPercentage = 1- Mathf.Clamp01(distancePlayerToBall / _startKickDistance);
			
			if (distanceBallToFoot < 0.2f)
			{
				_stopKicking = true;
				_stopKickTime = Time.time;
			}

			if (_stopKicking)
			{
				// lerp weights back
				_ikWeight = Mathf.Lerp(1, 0, (Time.time - _stopKickTime) / 0.5f);
				if (_ikWeight < 0.01f)
				{
					_stopKicking = false;
				}
			}
			
			_ikWeight = kickProgressPercentage;
		}

		[ContextMenu("Start kicking the ball")]
		public void StartKickingTheBall(Ball ball, float startKickDistance)
		{
			_stopKicking = false;
			_startKickDistance = startKickDistance;
			_ball = ball;
		}
		
		void OnAnimatorIK(int layerIndex)
		{
			if (!_ball)
				return;
			
			_animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _ikWeight);
			_animator.SetIKPosition(AvatarIKGoal.RightFoot, _ball.transform.position);
		}

		float _ikWeight;
		bool _stopKicking;
		float _stopKickTime;
	}
}
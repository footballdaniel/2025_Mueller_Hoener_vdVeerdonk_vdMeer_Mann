using System;
using Interactions.Domain.Goals;
using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class Opponent : MonoBehaviour
	{

		[SerializeField] Animator _animator;
		[SerializeField] float distanceFromAttacker = 3f;
		[SerializeField] float maxSpeed = 5f;
		[SerializeField] float maxAcceleration = 5f;
		[SerializeField] float maxRotationSpeedDegreesY = 90f;
		[SerializeField] float memoryDuration = 1f;
		[SerializeField] float reactionDelay = 0.3f;
		public event Action<Vector2> BallIntercepted;

		public Vector3 Position => transform.position;

		void Update()
		{
			_memory.Tick(Time.time);

			transform.position = _motor.Move(_sources, transform.position, transform.rotation, Time.deltaTime);
			transform.rotation = _motor.Rotate(_sources, transform.position, transform.rotation, Time.deltaTime);

			_animations.Apply(_motor.LocalVelocity);

			if (InterceptedBall())
			{
				BallIntercepted?.Invoke(_motor.Velocity);
				_sources.ActivateOnly(_attackerSource);
			}
		}

		public void Bind(User user, LeftGoal goalLeft, RightGoal goalRight)
		{
			_user = user;
			_goalLeft = goalLeft;
			_goalRight = goalRight;

			_memory = new DelayedPerceptionMemory(memoryDuration, reactionDelay, _user);
			_attackerSource = new AttackerInformationSource(_goalLeft.transform, _goalRight.transform, _memory, distanceFromAttacker);
			_footSource = new FootInformationSource(_user.DominantFoot, user.NonDominantFoot);
			_motor = new Motor(maxSpeed, maxAcceleration, maxRotationSpeedDegreesY);
			_animations = new Animations(_animator);
			_interceptionSource = new NoInterceptionInformationSource();

			_sources.Add(_attackerSource, 1f);
			_sources.Add(_footSource, 0.33f);
		}

		public void ChangeAcceleration(float newAcceleration)
		{
			_motor.ChangeAcceleration(newAcceleration);
		}

		public void ChangeInterpersonalDistance(float newInterpersonalDistance)
		{
			_attackerSource.ChangeInterpersonalDistance(newInterpersonalDistance);
		}

		public void ChangeReactionTime(float newReactionTime)
		{
			_memory.ChangeReactionTime(newReactionTime);
		}

		public void Intercept(Ball ball)
		{
			_interceptionSource = new InterceptionInformationSource(this, ball);
			_sources.ActivateOnly(_interceptionSource);
		}

		bool InterceptedBall()
		{
			return Vector3.Distance(transform.position, _interceptionSource.TargetPosition()) < 0.5f;
		}

		readonly InformationSources _sources = new();
		Animations _animations;
		AttackerInformationSource _attackerSource;
		FootInformationSource _footSource;
		LeftGoal _goalLeft;
		RightGoal _goalRight;
		IInformationSource _interceptionSource;
		DelayedPerceptionMemory _memory;
		Motor _motor;
		User _user;
	}

}
using System;
using Interactions.Domain.Goals;
using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class Opponent : MonoBehaviour
	{

		[SerializeField] Animator _animator;
		[SerializeField] float _distanceFromAttacker = 3f;
		[SerializeField] float _maxSpeed = 5f;
		[SerializeField] float _maxAcceleration = 5f;
		[SerializeField] float _maxRotationSpeedDegreesY = 90f;
		[SerializeField] float _memoryDuration = 1f;
		[SerializeField] float _reactionDelay = 0.3f;
		public event Action<Vector2> BallIntercepted;

		public Vector3 Position => transform.position;

		void Update()
		{
			_perception.Tick(Time.time);

			transform.position = _motor.Move(_sources, transform.position, transform.rotation, Time.deltaTime);
			transform.rotation = _motor.Rotate(_sources, transform.position, transform.rotation, Time.deltaTime);

			_animations.Apply(_motor.LocalVelocity);

			if (InterceptedBall())
			{
				BallIntercepted?.Invoke(_motor.Velocity);
				_sources.ActivateOnly(_attackerSource);
			}
		}

		public void Bind(User user, LeftGoal goalLeft, RightGoal goalRight, bool isInteractive)
		{
			_user = user;
			_goalLeft = goalLeft;
			_goalRight = goalRight;
			_isInteractive = isInteractive;
			
			if (isInteractive)
			{
				_perception = new DelayedAttackerPerception(_memoryDuration, _reactionDelay, _user);
				_footSource = new FootInformationSource(_user.DominantFoot, user.NonDominantFoot);
				_sources.Update(_footSource, 0.33f);
			}
			else
			{
				_perception = new InitialAttackerPerception(_user);
			}
			
			_attackerSource = new AttackerInformationSource(_goalLeft.transform, _goalRight.transform, _perception, _distanceFromAttacker);
			_motor = new Motor(_maxSpeed, _maxAcceleration, _maxRotationSpeedDegreesY);
			_animations = new Animations(_animator);
			_interceptionSource = new NoInterceptionInformationSource();
			
			_sources.Update(_attackerSource, 1f);
		}

		public void ChangeAcceleration(float newAcceleration)
		{
			_motor.ChangeAcceleration(newAcceleration);
		}

		public void ChangeBodyInformationWeight(float newWeight)
		{
			_sources.Update(_attackerSource, newWeight);
		}

		public void ChangeFootInformation(float newWeight)
		{
			_sources.Update(_footSource, newWeight);
		}

		public void ChangeInterpersonalDistance(float newInterpersonalDistance)
		{
			_attackerSource.ChangeInterpersonalDistance(newInterpersonalDistance);
		}

		public void ChangeReactionTime(float newReactionTime)
		{
			_perception.ChangeReactionTime(newReactionTime);
		}

		public void Intercept(Ball ball)
		{
			if (!_isInteractive) 
				return;
			
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
		IAttackerPerception _perception;
		Motor _motor;
		User _user;
		bool _isInteractive;
	}

}
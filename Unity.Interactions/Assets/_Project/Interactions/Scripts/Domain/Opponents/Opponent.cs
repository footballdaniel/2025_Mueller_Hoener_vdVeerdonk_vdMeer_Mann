using System;
using Interactions.Domain.DecisionMaking;
using Interactions.Domain.DecisionMaking.Constraints;
using Interactions.Domain.DecisionMaking.InformationCoupling;
using Interactions.Domain.DecisionMaking.Perceptions;
using Interactions.Domain.Goals;
using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class Opponent : MonoBehaviour
	{
		[SerializeField] BodyOrientation _bodyOrientation;
		[SerializeField] Legs _legs;
		[SerializeField] Animator _animator;
		[SerializeField] float _distanceFromAttacker = 3f;
		[SerializeField] float _maxSpeed = 5f;
		[SerializeField] float _maxAcceleration = 5f;
		[SerializeField] float _maxRotationSpeedDegreesY = 90f;
		[SerializeField] float _memoryDuration = 1f;
		[SerializeField] float _reactionDelayBody = 0.4f;
		[SerializeField] float _reactionDelayFoot = 0.4f;

		public Vector3 Position => transform.position;
		public Legs Legs => _legs;

		void Update()
		{
			if (!HasUserStartedTrial())
				return;

			_attackerPerception.Tick(Time.time);
			_footPerception.Tick(Time.time);

			transform.position = _motorController.Move(_sources, _opponentMaximalPositionConstraint, Time.deltaTime);
			transform.rotation = _motorController.Rotate(_sources, Time.deltaTime);

			_animations.Apply(_motorController.LocalVelocity);
		}

		public void FinishedKicking()
		{
			_sources.Remove(_interceptionSource);
			_sources.ActivateAll();
		}

		public void Bind(User user, LeftGoal goalLeft, RightGoal goalRight, OpponentMaximalPositionConstraint opponentMaximalPositionConstraint, bool isInteractive)
		{
			_user = user;
			_goalLeft = goalLeft;
			_goalRight = goalRight;
			_isInteractive = isInteractive;
			_opponentMaximalPositionConstraint = opponentMaximalPositionConstraint;
			_legs.Bind(this, user);

			if (isInteractive)
			{
				_attackerPerception = new DelayedAttackerPercept(_memoryDuration, _reactionDelayBody, _user);
				_footPerception = new DelayedFootPerception(_memoryDuration, _reactionDelayFoot, 0.4f, _user.DominantFoot, user.NonDominantFoot);
				_bodyOrientation.LookAt(_user.TrackedHead.transform);
			}
			else
			{
				_attackerPerception = new InitialPerceptWithGaussianNoise(_user);
				_footPerception = new NoFootPerception();
			}

			_footSource = new FootInformationSource(_footPerception);
			_attackerSource = new PutPressureOnAttackerSource(_goalLeft.transform, _goalRight.transform, _attackerPerception, _distanceFromAttacker);
			// _attackerSource = new MirroredPressureOnAttackerSource(_goalLeft.transform, _goalRight.transform, this, _attackerPerception, _distanceFromAttacker);
			_motorController = new MotorController(_maxSpeed, _maxAcceleration, _maxRotationSpeedDegreesY, transform.position, transform.rotation);
			_animations = new Animations(_animator);
			_interceptionSource = new NoInterceptionInformationSource();

			_sources.AddNewSource(_attackerSource, 1f);
			_sources.AddNewSource(_footSource, 0.33f);
		}

		public void ChangeAcceleration(float newAcceleration)
		{
			_motorController.ChangeAcceleration(newAcceleration);
		}

		public void ChangeBodyInformationWeight(float newWeight)
		{
			_sources.AddNewSource(_attackerSource, newWeight);
		}

		public void ChangeFootInformation(float newWeight)
		{
			_sources.AddNewSource(_footSource, newWeight);
		}

		public void ChangeInterpersonalDistance(float newInterpersonalDistance)
		{
			_attackerSource.ChangeInterpersonalDistance(newInterpersonalDistance);
		}

		public void ChangeReactionTimeBody(float newReactionTime)
		{
			_attackerPerception.ChangeReactionTime(newReactionTime);
		}

		public void ChangeReactionTimeFoot(float arg0)
		{
			_footPerception.ChangeReactionTime(arg0);
		}

		public void Intercept(Ball ball)
		{
			_bodyOrientation.LookStraightAhead();

			_interceptionSource = new InterceptionInformationSource(this, ball);
			_ball = ball;
			Debug.Log("Intercept command");
			_sources.ActivateOnly(_interceptionSource);
			
			_legs.StartKickingTheBall(_ball, 4f);
		}

		bool HasUserStartedTrial()
		{
			if (_hasUserStartedTrial)
				return true;
			
			_hasUserStartedTrial  =  _user.TrackedHead.transform.position.x > -2f;
			return _hasUserStartedTrial;
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(_sources.CombinePositions(), 0.5f);
		}

		readonly InformationSources _sources = new();
		Animations _animations;
		IPercept _attackerPerception;
		PutPressureOnAttackerSource _attackerSource;
		Ball _ball;
		IPercept _footPerception;
		IInformationSource _footSource;
		LeftGoal _goalLeft;
		RightGoal _goalRight;
		IInformationSource _interceptionSource;
		bool _isInteractive;
		MotorController _motorController;
		OpponentMaximalPositionConstraint _opponentMaximalPositionConstraint;
		User _user;
		bool _hasUserStartedTrial;
	}

}
using System;
using Interactions.Domain;
using Interactions.Domain.Opponents;
using UnityEngine;

public class Opponent : MonoBehaviour
{
	public event Action<Vector2> BallIntercepted;
	
	[SerializeField] Animator _animator;
	[SerializeField] float distanceFromAttacker = 3f;
	[SerializeField] float maxSpeed = 5f;
	[SerializeField] float acceleration = 5f;
	[SerializeField] float maxRotationSpeedDegreesY = 90f;
	[SerializeField] float memoryDuration = 1f;
	[SerializeField] float reactionDelay = 0.3f;

	public Vector3 Position => transform.position;

	void Start()
	{
		_memory = new DelayedPerceptionMemory(memoryDuration, reactionDelay);
		_attackerSource = new AttackerInformationSource(_goalLeft.transform, _goalRight.transform, _memory, distanceFromAttacker);
		_motor = new Motor(maxSpeed, acceleration, maxRotationSpeedDegreesY);
		_animations = new Animations(_animator);
		_interceptionSource = new NoInterceptionInformationSource();

		_sources.Add(_attackerSource, 1f);
	}

	void Update()
	{
		_memory.Add(Time.time, new Vector2(_user.transform.position.x, _user.transform.position.z));

		transform.position = _motor.Move(_sources, transform.position, transform.rotation, Time.deltaTime);
		transform.rotation = _motor.Rotate(_sources, transform.position, transform.rotation, Time.deltaTime);

		if (InterceptedBall())
		{
			BallIntercepted?.Invoke(_motor.Velocity);
			_sources.ActivateOnly(_attackerSource);
		}

		_animations.Apply(_motor.Velocity);
	}

	public void Bind(User user, LeftGoal goalLeft, RightGoal goalRight)
	{
		_user = user;
		_goalLeft = goalLeft;
		_goalRight = goalRight;
	}

	public void Intercept(Ball ball)
	{
		_interceptionSource = new InterceptionInformationSource(this, ball);
		_sources.ActivateOnly(_interceptionSource);
	}

	bool InterceptedBall()
	{
		return Vector3.Distance(transform.position, _interceptionSource.GetDesiredPosition()) < 0.5f;
	}

	readonly InformationSources _sources = new();
	Animations _animations;
	IInformationSource _attackerSource;
	LeftGoal _goalLeft;
	RightGoal _goalRight;
	IInformationSource _interceptionSource;
	DelayedPerceptionMemory _memory;
	Motor _motor;
	User _user;
}
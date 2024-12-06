using Interactions.Domain;
using Interactions.Domain.Opponents;
using UnityEngine;

public class Opponent : MonoBehaviour
{
	[SerializeField] Animator _animator;
	[SerializeField] float distanceFromAttacker = 3f;
	[SerializeField] float maxSpeed = 5f;
	[SerializeField] float acceleration = 5f;
	[SerializeField] float maxRotationSpeedDegreesY = 90f;
	[SerializeField] float memoryDuration = 1f;
	[SerializeField] float reactionDelay = 0.3f;

	void Start()
	{
		_memory = new DelayedPerceptionMemory(memoryDuration, reactionDelay);
		_attackerSource = new AttackerInformationSource(_goalLeft.transform, _goalRight.transform, _memory, distanceFromAttacker);
		_interceptionSource = new InterceptionInformationSource(transform, null);
		_motor = new Motor(maxSpeed, acceleration, maxRotationSpeedDegreesY);
		_animations = new Animations(_animator);

		_sources.Add(_attackerSource, 1f);
		_sources.Add(_interceptionSource, 0f);
	}

	void Update()
	{
		_memory.Add(Time.time, new Vector2(_user.transform.position.x, _user.transform.position.z));
		
		transform.position = _motor.Move(_sources, transform.position, transform.rotation, Time.deltaTime);
		transform.rotation = _motor.Rotate(_sources, transform.position, transform.rotation, Time.deltaTime);

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
		_interceptionSource = new InterceptionInformationSource(transform, ball);
		_sources.ActivateOnly(_interceptionSource);
	}

	readonly InformationSources _sources = new();
	Animations _animations;
	IInformationSource _attackerSource;
	Vector3 _currentVelocity;
	LeftGoal _goalLeft;
	RightGoal _goalRight;
	IInformationSource _interceptionSource;
	DelayedPerceptionMemory _memory;
	Motor _motor;

	User _user;
}
using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class AttackerInformationSource : IInformationSource
	{

		public AttackerInformationSource(Transform goalLeft, Transform goalRight, DelayedPerceptionMemory memory, float distanceFromAttacker)
		{
			_goalLeft = goalLeft;
			_goalRight = goalRight;
			_memory = memory;
			_distanceFromAttacker = distanceFromAttacker;
		}

		public Vector3 TargetPosition()
		{
			var positionBetweenGoals = (_goalLeft.position + _goalRight.position) / 2;
			var pos = _memory.Get(Time.time);
			var userPosition = new Vector3(pos.x, 0, pos.y);
			var dir = (userPosition - positionBetweenGoals).normalized;
			return userPosition - dir * _distanceFromAttacker;
		}

		public float TargetRotationY()
		{
			var positionBetweenGoals = (_goalLeft.position + _goalRight.position) / 2;
			var pos = _memory.Get(Time.time);
			var userPosition = new Vector3(pos.x, 0, pos.y);
			var dir = (userPosition - positionBetweenGoals).normalized;
			return Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
		}

		public float Weight { get; set; }
		float _distanceFromAttacker;
		Transform _goalLeft;
		Transform _goalRight;
		DelayedPerceptionMemory _memory;
	}
}
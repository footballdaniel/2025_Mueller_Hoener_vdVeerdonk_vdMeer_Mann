using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class AttackerInformationSource : IInformationSource
	{

		public AttackerInformationSource(Transform goalLeft, Transform goalRight, IPercept percept, float distanceFromAttacker)
		{
			_goalLeft = goalLeft;
			_goalRight = goalRight;
			_percept = percept;
			_distanceFromAttacker = distanceFromAttacker;
		}
		
		public void ChangeInterpersonalDistance(float distance)
		{
			_distanceFromAttacker = distance;
		}

		public Vector3 TargetPosition()
		{
			var positionBetweenGoals = (_goalLeft.position + _goalRight.position) / 2;
			var pos = _percept.Perceive();
			var userPosition = new Vector3(pos.x, 0, pos.y);
			var dir = (userPosition - positionBetweenGoals).normalized;
			return userPosition - dir * _distanceFromAttacker;
		}

		public float TargetRotationY()
		{
			var positionBetweenGoals = (_goalLeft.position + _goalRight.position) / 2;
			var pos = _percept.Perceive();
			var userPosition = new Vector3(pos.x, 0, pos.y);
			var dir = (userPosition - positionBetweenGoals).normalized;
			return Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
		}

		public float Weight { get; set; }
		float _distanceFromAttacker;
		readonly Transform _goalLeft;
		readonly Transform _goalRight;
		readonly IPercept _percept;
	}
}
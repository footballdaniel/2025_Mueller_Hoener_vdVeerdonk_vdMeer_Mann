using Interactions.Domain.DecisionMaking.Perceptions;
using UnityEngine;

namespace Interactions.Domain.DecisionMaking.InformationCoupling
{
	public class PutPressureOnAttackerSource : IInformationSource
	{

		public PutPressureOnAttackerSource(Transform goalLeft, Transform goalRight, IPercept perceptionOfAttacker, float desiredInterpersonalDistance)
		{
			_goalLeft = goalLeft;
			_goalRight = goalRight;
			_perceptionOfAttacker = perceptionOfAttacker;
			_desiredInterpersonalDistance = desiredInterpersonalDistance;
		}
		
		public void ChangeInterpersonalDistance(float distance)
		{
			_desiredInterpersonalDistance = distance;
		}

		public Vector3 TargetPosition()
		{
			var positionBetweenGoals = (_goalLeft.position + _goalRight.position) / 2;
			var pos = _perceptionOfAttacker.Perceive();
			var userPosition = new Vector3(pos.x, 0, pos.y);
			var dir = (userPosition - positionBetweenGoals).normalized;
			return userPosition - dir * _desiredInterpersonalDistance;
		}

		public float TargetRotationY()
		{
			var positionBetweenGoals = (_goalLeft.position + _goalRight.position) / 2;
			var pos = _perceptionOfAttacker.Perceive();
			var userPosition = new Vector3(pos.x, 0, pos.y);
			var dir = (userPosition - positionBetweenGoals).normalized;
			return Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
		}

		public float Weight { get; set; }
		float _desiredInterpersonalDistance;
		readonly Transform _goalLeft;
		readonly Transform _goalRight;
		readonly IPercept _perceptionOfAttacker;
	}
}
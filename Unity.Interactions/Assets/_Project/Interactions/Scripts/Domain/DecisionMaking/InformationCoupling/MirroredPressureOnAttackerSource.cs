using System;
using Interactions.Domain.DecisionMaking.Perceptions;
using Interactions.Domain.Opponents;
using UnityEngine;

namespace Interactions.Domain.DecisionMaking.InformationCoupling
{
	public class MirroredPressureOnAttackerSource : IInformationSource
	{

		public MirroredPressureOnAttackerSource(Transform goalLeft, Transform goalRight, Opponent opponent, IPercept perceptionOfAttacker, float desiredInterpersonalDistance)
		{
			_goalLeft = goalLeft;
			_goalRight = goalRight;
			_perceptionOfAttacker = perceptionOfAttacker;
			_desiredInterpersonalDistance = desiredInterpersonalDistance;
			_opponent = opponent;
		}
		
		public void ChangeInterpersonalDistance(float distance)
		{
			_desiredInterpersonalDistance = distance;
		}

		public Vector3 TargetPosition()
		{
			var positionBetweenGoals = (_goalLeft.position + _goalRight.position) / 2;
			var pos = _perceptionOfAttacker.Perceive();
			var attackerPosition = new Vector3(pos.x, 0, pos.y);
			var dir = (attackerPosition - positionBetweenGoals).normalized;
			// the point where the defender will finally reach
			var interpersonalPoint =  attackerPosition - dir * _desiredInterpersonalDistance;
			
			
			// From now on progress towards the point
			var vectorFromOpponentToInterpersonalPoint = interpersonalPoint - _opponent.Position;
			var distanceFromOpponentToInterpersonalPoint = vectorFromOpponentToInterpersonalPoint.magnitude;
			var directionFromOpponentToInterpersonalPoint = vectorFromOpponentToInterpersonalPoint.normalized;

			var maximalPogress = Math.Min(_totalXMovement, distanceFromOpponentToInterpersonalPoint);
			var progressingFromOpponentTowardsInterpersonalPoint = _opponent.Position + directionFromOpponentToInterpersonalPoint * maximalPogress;
			
			// cache for next frame
			if (_lastAttackerPosition.magnitude == 0)
				_lastAttackerPosition = pos;
			
			_totalXMovement += Mathf.Abs(_lastAttackerPosition.x - pos.x);
			_lastAttackerPosition = pos;
			
			return progressingFromOpponentTowardsInterpersonalPoint;
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
		Vector2 _lastAttackerPosition;
		float _totalXMovement;
		readonly Opponent _opponent;
	}
}
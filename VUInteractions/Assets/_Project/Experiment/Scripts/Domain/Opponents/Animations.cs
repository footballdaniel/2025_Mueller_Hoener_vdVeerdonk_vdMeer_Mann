using UnityEngine;

namespace Interactions.Domain.Opponents
{
	public class Animations
	{

		public Animations(Animator animator)
		{
			_animator = animator;
		}

		public void Apply(Vector2 velocity)
		{
			_animator.SetFloat(VelocityX, velocity.x);
			_animator.SetFloat(VelocityY, velocity.y);
		}

		readonly static int VelocityX = Animator.StringToHash("VelocityX");
		readonly static int VelocityY = Animator.StringToHash("VelocityY");
		readonly Animator _animator;
	}
}
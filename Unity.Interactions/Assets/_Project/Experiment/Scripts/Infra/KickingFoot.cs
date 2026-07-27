using UnityEngine;

namespace Interactions.Infra
{
	/// The peak velocity observed in the window together with the foot that produced it.
	/// The pass detection model only reports that a pass happened, not which foot made it,
	/// so the kicking foot is identified as the faster of the two - at the moment of a pass
	/// the kicking foot far outruns the planted one.
	public readonly struct KickingFoot
	{
		public KickingFoot(Vector3 velocity, bool isDominantFoot)
		{
			Velocity = velocity;
			IsDominantFoot = isDominantFoot;
		}

		public Vector3 Velocity { get; }
		public bool IsDominantFoot { get; }
	}
}

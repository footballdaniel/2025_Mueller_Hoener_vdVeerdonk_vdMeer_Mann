using UnityEngine;

namespace Interactions.Domain.Tracking
{
	/// A view into bone data owned by the tracking source — copying a BodyPose
	/// copies two references and an int, never the bone data itself. Bones are
	/// laid out per (int)Bone starting at the offset. Bones without recorded
	/// data return Vector3.zero / Quaternion.identity.
	public readonly struct BodyPose
	{
		public BodyPose(Vector3[] positions, Quaternion[] rotations = null, int offset = 0)
		{
			_positions = positions;
			_rotations = rotations;
			_offset = offset;
		}

		public Vector3 Position(Bone bone)
		{
			var index = _offset + (int)bone;
			return _positions != null && index < _positions.Length ? _positions[index] : Vector3.zero;
		}

		public Quaternion Rotation(Bone bone)
		{
			var index = _offset + (int)bone;
			return _rotations != null && index < _rotations.Length ? _rotations[index] : Quaternion.identity;
		}

		readonly int _offset;
		readonly Vector3[] _positions;
		readonly Quaternion[] _rotations;
	}
}

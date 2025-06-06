using UnityEngine;

public class CalibrationData
{
	public CalibrationData(Vector3 motionTrackerPosition, Quaternion motionTrackerRotation)
	{
		Offset = motionTrackerPosition;
		Rotation = motionTrackerRotation;
	}

	public Vector3 Offset { get; set; }
	public Quaternion Rotation { get; set; }
}
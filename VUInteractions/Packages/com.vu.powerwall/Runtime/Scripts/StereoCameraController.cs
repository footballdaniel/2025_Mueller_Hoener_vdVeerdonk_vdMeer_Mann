using UnityEngine;

public class StereoCameraController : MonoBehaviour
{
	[Header("External dependencies"), SerializeField] XRTracker _motionTracker;
	[SerializeField] ProjectionPlane _projectionPlane;

	[Header("Internal dependencies"), SerializeField] Camera _cameraLeft;
	[SerializeField] Camera _cameraRight;
	[SerializeField] ProjectionMatrix _projectionPlaneLeft;
	[SerializeField] ProjectionMatrix _projectionPlaneRight;

	bool _active3D;
	bool _displayLeftEyeImage;

	void Awake()
	{
		Application.targetFrameRate = 120;
	}

	void Start()
	{
		transform.SetParent(_motionTracker.transform);
		transform.localPosition = Vector3.zero;

		_projectionPlaneLeft.Set(_projectionPlane.gameObject);
		_projectionPlaneRight.Set(_projectionPlane.gameObject);
	}

	void FixedUpdate()
	{
		if (_active3D)
			SwitchBetweenCameras();
	}

	void SwitchBetweenCameras()
	{
		if (_displayLeftEyeImage)
		{
			_cameraLeft.Render();
			_cameraLeft.enabled = true;
			_cameraRight.enabled = false;
			_displayLeftEyeImage = false;
		}
		else
		{
			_cameraRight.Render();
			_cameraRight.enabled = true;
			_cameraLeft.enabled = false;
			_displayLeftEyeImage = true;
		}
	}
}
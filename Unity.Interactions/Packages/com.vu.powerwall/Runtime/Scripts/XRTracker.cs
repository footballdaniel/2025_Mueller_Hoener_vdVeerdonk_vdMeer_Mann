using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

[Serializable]
public class XRTracker : MonoBehaviour
{
	[SerializeField] InputActionReference _positionAction;
	[SerializeField] bool _isXForward;

	[field: SerializeReference] public string TrackerMappingName { get; private set; }

	public Vector3 Position { get; set; }

	public bool IsTracked
	{
		get
		{
			var action = _positionAction != null ? _positionAction.action : null;
			if (action == null || action.controls.Count == 0)
				return false;

			return action.controls[0].device is TrackedDevice device && device.isTracked.isPressed;
		}
	}

	void OnEnable()
	{
		_positionAction.action.Enable();
	}

	void OnDisable()
	{
		_positionAction.action.Disable();
	}


	void Update()
	{
		Position = _positionAction.action.ReadValue<Vector3>();

		if (_isXForward)
			transform.localPosition = new Vector3(Position.z, Position.y, -Position.x);
		else
			transform.localPosition = Position;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, transform.forward);

		// gizmo in z direction
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(transform.position, transform.up);
	}
}
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class XRTracker : MonoBehaviour
{
	[SerializeField] InputActionReference _positionAction;
	[SerializeField] bool _isXForward;

	[field: SerializeReference] public string TrackerMappingName { get; private set; }

	public Vector3 Position { get; set; }

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
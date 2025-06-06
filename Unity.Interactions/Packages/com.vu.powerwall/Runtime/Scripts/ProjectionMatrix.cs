using UnityEngine;

[ExecuteInEditMode]
public class ProjectionMatrix : MonoBehaviour
{
	[SerializeField] GameObject _projectionScreen;
	[SerializeField] Camera _camera;


	void LateUpdate()
	{
		if (!_projectionScreen || !_camera)
			return;

		// Reset the projection and world to camera matrices
		_camera.ResetProjectionMatrix();
		_camera.ResetWorldToCameraMatrix();

		var pa = _projectionScreen.transform.TransformPoint(new Vector3(-5.0f, 0.0f, -5.0f));
		// lower left corner in world coordinates
		var pb = _projectionScreen.transform.TransformPoint(new Vector3(5.0f, 0.0f, -5.0f));
		// lower right corner
		var pc = _projectionScreen.transform.TransformPoint(new Vector3(-5.0f, 0.0f, 5.0f));


		// upper left corner
		var pe = transform.position;
		// eye position
		var n = _camera.nearClipPlane;
		// distance of near clipping plane
		var f = _camera.farClipPlane;
		// distance of far clipping plane

		Vector3 va; // from pe to pa
		Vector3 vb; // from pe to pb
		Vector3 vc; // from pe to _pc
		Vector3 vr; // right axis of screen
		Vector3 vu; // up axis of screen
		Vector3 vn; // normal vector of screen

		float l; // distance to left screen edge
		float r; // distance to right screen edge
		float b; // distance to bottom screen edge
		float t; // distance to top screen edge
		float d; // distance from eye to screen 

		vr = pb - pa;
		vu = pc - pa;
		vr.Normalize();
		vu.Normalize();
		vn = -Vector3.Cross(vr, vu);
		// we need the minus sign because Unity 
		// uses a left-handed coordinate system
		vn.Normalize();

		va = pa - pe;
		vb = pb - pe;
		vc = pc - pe;

		d = -Vector3.Dot(va, vn);
		l = Vector3.Dot(vr, va) * n / d;
		r = Vector3.Dot(vr, vb) * n / d;
		b = Vector3.Dot(vu, va) * n / d;
		t = Vector3.Dot(vu, vc) * n / d;

		// projection matrix
		var p = new Matrix4x4
		{
			[0, 0] = 2.0f * n / (r - l),
			[0, 1] = 0.0f,
			[0, 2] = (r + l) / (r - l),
			[0, 3] = 0.0f,
			[1, 0] = 0.0f,
			[1, 1] = 2.0f * n / (t - b),
			[1, 2] = (t + b) / (t - b),
			[1, 3] = 0.0f,
			[2, 0] = 0.0f,
			[2, 1] = 0.0f,
			[2, 2] = (f + n) / (n - f),
			[2, 3] = 2.0f * f * n / (n - f),
			[3, 0] = 0.0f,
			[3, 1] = 0.0f,
			[3, 2] = -1.0f,
			[3, 3] = 0.0f
		};

		// rotation matrix
		var rm = new Matrix4x4
		{
			[0, 0] = vr.x,
			[0, 1] = vr.y,
			[0, 2] = vr.z,
			[0, 3] = 0.0f,
			[1, 0] = vu.x,
			[1, 1] = vu.y,
			[1, 2] = vu.z,
			[1, 3] = 0.0f,
			[2, 0] = vn.x,
			[2, 1] = vn.y,
			[2, 2] = vn.z,
			[2, 3] = 0.0f,
			[3, 0] = 0.0f,
			[3, 1] = 0.0f,
			[3, 2] = 0.0f,
			[3, 3] = 1.0f
		};

		// translation matrix;
		var tm = new Matrix4x4
		{
			[0, 0] = 1.0f,
			[0, 1] = 0.0f,
			[0, 2] = 0.0f,
			[0, 3] = -pe.x,
			[1, 0] = 0.0f,
			[1, 1] = 1.0f,
			[1, 2] = 0.0f,
			[1, 3] = -pe.y,
			[2, 0] = 0.0f,
			[2, 1] = 0.0f,
			[2, 2] = 1.0f,
			[2, 3] = -pe.z,
			[3, 0] = 0.0f,
			[3, 1] = 0.0f,
			[3, 2] = 0.0f,
			[3, 3] = 1.0f
		};


		// Check if the projection matrix contains valid values
		for (var i = 0; i < 4; i++)
		for (var j = 0; j < 4; j++)
			if (float.IsNaN(p[i, j]) || float.IsInfinity(p[i, j]))
				return;

		// Set the projection matrix and world to camera matrix
		_camera.projectionMatrix = p;
		_camera.worldToCameraMatrix = rm * tm;
	}

	public void Set(GameObject projectionPlaneGameObject)
	{
		_projectionScreen = projectionPlaneGameObject;
	}
}
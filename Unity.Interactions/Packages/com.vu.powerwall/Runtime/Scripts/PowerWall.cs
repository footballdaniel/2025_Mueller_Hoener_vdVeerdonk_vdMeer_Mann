using UnityEngine;

public class PowerWall : MonoBehaviour
{
	[Header("Dependencies"), SerializeField] MeshFilter _projectionPlane;

	#region Gizmos

	void OnDrawGizmos()
	{
		Gizmos.DrawWireMesh(
			_projectionPlane.sharedMesh,
			_projectionPlane.transform.position,
			_projectionPlane.transform.rotation,
			_projectionPlane.transform.localScale);


		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, transform.forward);
	}

	#endregion

}
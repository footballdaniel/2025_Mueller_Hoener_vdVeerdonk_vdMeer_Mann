using UnityEngine;

public class Goal : MonoBehaviour
{
	[SerializeField] MeshRenderer _meshRenderer;

	void OnEnable()
	{
		_meshRenderer.enabled = false;
	}


	public void NoFeedback()
	{
		_meshRenderer.enabled = false;
	}
	
	public void Score()
	{
		_meshRenderer.enabled = true;
		_meshRenderer.material.color = Color.green;
	}
	
	public void Miss()
	{
		_meshRenderer.enabled = true;
		_meshRenderer.material.color = Color.red;
	}
}
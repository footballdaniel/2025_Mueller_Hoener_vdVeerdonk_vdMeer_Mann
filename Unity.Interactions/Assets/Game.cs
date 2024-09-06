using UnityEngine;

public class Game : MonoBehaviour
{
	void Start()
	{
		ServiceLocator.Get<WebcamRecorder>();
	}


	void Update()
	{
	}
}
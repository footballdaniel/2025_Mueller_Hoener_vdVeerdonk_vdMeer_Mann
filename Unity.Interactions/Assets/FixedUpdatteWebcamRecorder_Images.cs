using System.IO;
using UnityEngine;

namespace _Project
{
	public class FixedUpdatteWebcamRecorder_Images : MonoBehaviour
	{
		void Start()
		{
			var  devices = WebCamTexture.devices;
			foreach (var device in devices)
				Debug.Log(device.name);
			
			
			_webcamTexture = new WebCamTexture(devices[1].name);
			_webcamTexture.Play();
			_tex = new Texture2D(_webcamTexture.width, _webcamTexture.height);
		}

		void FixedUpdate()
		{
			if (_webcamTexture.didUpdateThisFrame)
			{
				_frameIndex++;
				WriteFrameAsJpeg();
				Debug.Log(Time.timeSinceLevelLoad + " " + _frameIndex);
			}
		}

		void OnDestroy()
		{
			_webcamTexture?.Stop();
		}

		void WriteFrameAsJpeg()
		{
			_tex.SetPixels32(_webcamTexture.GetPixels32());
			_tex.Apply();

			var im = _tex.EncodeToJPG(75); // JPEG quality set to 75

			var filePath = Path.Combine(Application.persistentDataPath, $"frame_{_frameIndex}.jpg");
			File.WriteAllBytes(filePath, im);
		}

		int _frameIndex = 0;
		WebCamTexture _webcamTexture;
		Texture2D _tex;
	}
}
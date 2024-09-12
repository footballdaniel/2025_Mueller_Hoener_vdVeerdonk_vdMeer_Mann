using System.IO;
using UnityEngine;

namespace _Project
{
	public class FixedUpdatteWebcamRecorder_Blit : MonoBehaviour
	{
		void Start()
		{
			var devices = WebCamTexture.devices;
			foreach (var device in devices)
				Debug.Log(device.name);

			_webcamTexture = new WebCamTexture(devices[1].name);
			_webcamTexture.Play();

			// Initialize the RenderTexture
			_renderTexture = new RenderTexture(_webcamTexture.width, _webcamTexture.height, 0);
		}

		void FixedUpdate()
		{
			// Update at 5Hz (0.2 second interval)
			_updateTimer += Time.fixedDeltaTime;
			if (_updateTimer >= 0.2f)
			{
				_updateTimer = 0f;
				_frameIndex++;
				CaptureFrame();
				Debug.Log(Time.timeSinceLevelLoad + " " + _frameIndex);
			}
		}

		void OnDestroy()
		{
			_webcamTexture?.Stop();
		}

		void CaptureFrame()
		{
			// Use Graphics.Blit to copy the WebCamTexture to the RenderTexture
			Graphics.Blit(_webcamTexture, _renderTexture);

			// Synchronously read pixels from the RenderTexture
			RenderTexture.active = _renderTexture;
			_tex = new Texture2D(_renderTexture.width, _renderTexture.height, TextureFormat.RGB24, false);
			_tex.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
			_tex.Apply();
			RenderTexture.active = null;

			// Encode the texture and save the frame
			SaveFrame(_tex.EncodeToJPG(75));
		}

		void SaveFrame(byte[] imageData)
		{
			var filePath = Path.Combine(Application.persistentDataPath, $"frame_{_frameIndex}.jpg");
			File.WriteAllBytes(filePath, imageData);
		}

		int _frameIndex = 0;
		WebCamTexture _webcamTexture;
		RenderTexture _renderTexture;
		Texture2D _tex;
		float _updateTimer = 0f;  // Timer for updating at 5Hz
	}
}
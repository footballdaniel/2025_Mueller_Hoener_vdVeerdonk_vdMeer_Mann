using System.IO;
using UnityEngine;

namespace _Project
{
	public class FixedUpdatteWebcamRecorder_BlitWithText : MonoBehaviour
	{
		void Start()
		{
			var devices = WebCamTexture.devices;
			foreach (var device in devices)
				Debug.Log(device.name);

			_webcamTexture = new WebCamTexture(devices[1].name);
			_webcamTexture.Play();

			_renderTexture = new RenderTexture(_webcamTexture.width, _webcamTexture.height, 0);
		}

		void FixedUpdate()
		{
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
			Graphics.Blit(_webcamTexture, _renderTexture);
			RenderTexture.active = _renderTexture;
			_tex = new Texture2D(_renderTexture.width, _renderTexture.height, TextureFormat.RGB24, false);
			_tex.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
			_tex.Apply();
			RenderTexture.active = null;
			SaveFrame(_tex.EncodeToJPG(75));
		}

		void SaveFrame(byte[] imageData)
		{
			var filePath = Path.Combine(Application.persistentDataPath, $"frame_{_frameIndex}.jpg");
			File.WriteAllBytesAsync(filePath, imageData);
		}

		int _frameIndex = 0;
		WebCamTexture _webcamTexture;
		RenderTexture _renderTexture;
		Texture2D _tex;
		float _updateTimer = 0f; 
	}
}
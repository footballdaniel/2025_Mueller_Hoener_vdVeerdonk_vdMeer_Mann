using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Project
{
	public class FixedUpdatteWebcamRecorder_Blit_Async : MonoBehaviour
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
				CaptureFrameAsync();
				Debug.Log(Time.timeSinceLevelLoad + " " + _frameIndex);
			}
		}

		void OnDestroy()
		{
			_webcamTexture.Stop();
		}

		void CaptureFrameAsync()
		{
			// Use Graphics.Blit to copy the WebCamTexture to the RenderTexture
			Graphics.Blit(_webcamTexture, _renderTexture);

			// Use AsyncGPUReadback to capture the pixels asynchronously
			AsyncGPUReadback.Request(_renderTexture, 0, TextureFormat.RGB24, OnCompleteReadback);
		}

		void OnCompleteReadback(AsyncGPUReadbackRequest request)
		{
			if (request.hasError)
			{
				Debug.LogError("GPU readback error detected.");
				return;
			}

			var rawData = request.GetData<byte>();
			Task.Run(() => EncodeAndSaveFrame(rawData.ToArray(), _renderTexture.width, _renderTexture.height, _frameIndex));
		}

		void EncodeAndSaveFrame(byte[] rawData, int width, int height, int frameIndex)
		{
			var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
			tex.LoadRawTextureData(rawData);
			tex.Apply();

			var im = tex.EncodeToJPG(75);
			var filePath = Path.Combine(Application.persistentDataPath, $"frame_{frameIndex}.jpg");

			File.WriteAllBytes(filePath, im);
		}

		int _frameIndex = 0;
		WebCamTexture _webcamTexture;
		RenderTexture _renderTexture;
		float _updateTimer;
	}
}
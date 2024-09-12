using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace _Project
{
	public class FixedUpdatteWebcamRecorder_Async : MonoBehaviour
	{
		void Start()
		{
			var  devices = WebCamTexture.devices;
			foreach (var device in devices)
				Debug.Log(device.name);
			
			_webcamTexture = new WebCamTexture(devices[1].name);
			
			_webcamTexture.Play();
		}
		
		void FixedUpdate()
		{
			if (_webcamTexture.didUpdateThisFrame)
			{
				_frameIndex++;
				WriteFrameAsJpegAsync();
				Debug.Log(Time.timeSinceLevelLoad + " " + _frameIndex);
			}
		}

		void OnDestroy()
		{
			_webcamTexture?.Stop();
		}

		async void WriteFrameAsJpegAsync()
		{
			_webcamTexture.GetNativeTexturePtr();
			_tex = new Texture2D(_webcamTexture.width, _webcamTexture.height);
			_tex.SetPixels32(_webcamTexture.GetPixels32());
			_tex.Apply();

			var im = _tex.EncodeToJPG(75);
			var filePath = Path.Combine(Application.persistentDataPath, $"frame_{_frameIndex}.jpg");
			await File.WriteAllBytesAsync(filePath, im);
		}

		int _frameIndex = 0;
		float _captureTimer = 0f;
		WebCamTexture _webcamTexture;
		Texture2D _tex;
	}
}
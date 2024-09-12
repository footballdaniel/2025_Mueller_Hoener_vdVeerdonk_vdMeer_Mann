using System;
using System.IO;
using UnityEngine;

public class FixedUpdateWebcamRecorder_Stream : MonoBehaviour
{
	void Start()
	{
		_webcamTexture = new WebCamTexture();
		_webcamTexture.Play();
		var path = Path.Combine(Application.persistentDataPath, "CapturedFrames.bin");

		_fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
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
		_fileStream?.Close();
	}

	void WriteFrameAsJpeg()
	{
		var tex = new Texture2D(_webcamTexture.width, _webcamTexture.height);
		tex.SetPixels32(_webcamTexture.GetPixels32());
		tex.Apply();

		var im = tex.EncodeToJPG(75); // JPEG quality set to 75
		_fileStream.Write(BitConverter.GetBytes(im.Length), 0, sizeof(int));
		_fileStream.Write(im, 0, im.Length);
		_fileStream.Flush();
	}

	FileStream _fileStream;
	int _frameIndex = 0;
	WebCamTexture _webcamTexture;
}
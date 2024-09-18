using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class AvailableWebCams : List<WebCamConfiguration>
{
	public AvailableWebCams()
	{
		var devices = WebCamTexture.devices;
		foreach (var device in devices)
		{
			var config = new WebCamConfiguration(device.name, 1920, 1080, 30);
			Add(config);
		}
	}
}

public record WebCamConfiguration(string DeviceName, int Width, int Height, int FrameRate);

public class FixedUpdateWebcamRecorder_Stream : MonoBehaviour
{
	[SerializeField] float _frameRate = 5f;
	
	void Start()
	{
		var devices = WebCamTexture.devices;
		foreach (var device in devices)
			Debug.Log(device.name);
		_webcamTexture = new WebCamTexture(devices[1].name);

		_webcamTexture.Play();
		_frameFolderPath = Path.Combine(Application.persistentDataPath, "CapturedFrames");
		if (!Directory.Exists(_frameFolderPath))
			Directory.CreateDirectory(_frameFolderPath);
	}

	void FixedUpdate()
	{
		var deltaTime = 1f / _frameRate;
		_updateTimer += Time.fixedDeltaTime;
		float epsilon = 0.0001f; // Small value to account for floating-point errors

		if (_updateTimer >= deltaTime - epsilon)
		{
			_updateTimer -= deltaTime; // Subtract instead of resetting to zero
			_frameIndex++;
			SaveFrameAsPng();
			Debug.Log(Time.timeSinceLevelLoad + " " + _frameIndex);
		}
	}

	void OnDestroy()
	{
		_webcamTexture?.Stop();
		GenerateVideoWithFfmpeg();
	}

	void SaveFrameAsPng()
	{
		var tex = new Texture2D(_webcamTexture.width, _webcamTexture.height);
		tex.SetPixels32(_webcamTexture.GetPixels32());
		tex.Apply();

		var frameBytes = tex.EncodeToPNG();
		var fileName = Path.Combine(_frameFolderPath, $"frame_{_frameIndex:D6}.png");
		File.WriteAllBytes(fileName, frameBytes);
	}

	void GenerateVideoWithFfmpeg()
	{
		var ffmpegPath = Path.Combine(Application.streamingAssetsPath, "ffmpeg", "ffmpeg.exe");  // Ensure ffmpeg is placed here
		var videoOutputPath = Path.Combine(Application.persistentDataPath, "output_video.mp4");
		var arguments = $"-r {_frameRate} -i \"{_frameFolderPath}/frame_%06d.png\" -vf \"drawtext=fontfile=/path/to/font.ttf:text='%{{n}}':x=(w-tw)/2:y=h-th-10:fontsize=24:fontcolor=white\" -vcodec libx264 -crf 18 -pix_fmt yuv420p -y \"{videoOutputPath}\"";

		var process = new System.Diagnostics.Process();
		process.StartInfo.FileName = ffmpegPath;
		process.StartInfo.Arguments = arguments;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.CreateNoWindow = true;
		process.Start();
		// process.WaitForExit();
	}

	int _frameIndex = 0;
	string _frameFolderPath;
	WebCamTexture _webcamTexture;
	float _updateTimer;
}
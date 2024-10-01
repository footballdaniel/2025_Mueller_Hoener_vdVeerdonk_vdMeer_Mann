using System.Diagnostics;
using System.IO;
using Domain.VideoRecorder;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class FFMpegWebcamRecorder : MonoBehaviour, IWebcamRecorder
{
	[SerializeField] float _frameRate = 5f;

	public bool IsRecording => _webcamTexture is { isPlaying: true } && _isrecording;
	public bool IsExportComplete => _isExportComplete;

	public void StartRecording()
	{
		_isrecording = true;
	}

	public void StopRecording()
	{
		_isrecording = false;
		_webcamTexture.Stop();
	}

	public void Export()
	{
		GenerateVideoWithFfmpeg();
		_isExportComplete = true;
	}

	public void Set(WebCamConfiguration config)
	{
		_webcamTexture = new WebCamTexture(config.DeviceName, config.Height, config.Width, (int)config.FrameRate);

		_webcamTexture.Play();
		_frameFolderPath = Path.Combine(Application.persistentDataPath, "CapturedFrames");

		if (!Directory.Exists(_frameFolderPath))
			Directory.CreateDirectory(_frameFolderPath);
	}

	void FixedUpdate()
	{
		if (!IsRecording) return;

		var deltaTime = 1f / _frameRate;
		_updateTimer += Time.fixedDeltaTime;
		var epsilon = 0.0001f; // Small value to account for floating-point errors

		if (_updateTimer >= deltaTime - epsilon)
		{
			_updateTimer -= deltaTime; // Subtract instead of resetting to zero
			_frameIndex++;
			SaveFrameAsPng();
			Debug.Log(Time.timeSinceLevelLoad + " " + _frameIndex);
		}
	}

	void GenerateVideoWithFfmpeg()
	{
		var ffmpegPath = Path.Combine(Application.streamingAssetsPath, "ffmpeg", "ffmpeg.exe"); // Ensure ffmpeg is placed here
		var videoOutputPath = Path.Combine(Application.persistentDataPath, "output_video.mp4");
		var arguments = $"-r {_frameRate} -i \"{_frameFolderPath}/frame_%06d.png\" -vf \"drawtext=fontfile=/path/to/font.ttf:text='%{{n}}':x=(w-tw)/2:y=h-th-10:fontsize=24:fontcolor=white\" -vcodec libx264 -crf 18 -pix_fmt yuv420p -y \"{videoOutputPath}\"";

		var process = new Process();
		process.StartInfo.FileName = ffmpegPath;
		process.StartInfo.Arguments = arguments;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.CreateNoWindow = true;
		process.Start();
		// process.WaitForExit();
		_isExportComplete = true;
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

	string _frameFolderPath;

	int _frameIndex = 0;
	bool _isExportComplete;
	bool _isrecording;
	float _updateTimer;
	WebCamTexture _webcamTexture;
}
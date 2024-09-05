using UnityEngine;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

public class WebcamToVideo : MonoBehaviour, IService<WebcamToVideo>
{
	WebCamTexture _webCamTexture;
	Texture2D _frameTexture;
	int _frameCount;
	string _outputFolder;
	string _ffmpegPath;

	void Start()
	{
		var device = WebCamTexture.devices[1];
		_webCamTexture = new WebCamTexture(device.name, 640, 480);
		_webCamTexture.Play();

		_outputFolder = Path.Combine(Application.persistentDataPath, "CapturedFrames");
		if (!Directory.Exists(_outputFolder))
		{
			Directory.CreateDirectory(_outputFolder);
		}

		// Correctly format path to ffmpeg in Plugins directory
		_ffmpegPath = Path.Combine(Application.dataPath, "_Project", "Plugins", "ffmpeg.exe").Replace("/", "\\");
		
		var ffmpegFile = new FileInfo(_ffmpegPath);
		// exists?
		if (!ffmpegFile.Exists)
		{
			Debug.LogError("ffmpeg.exe not found at path: " + _ffmpegPath);
		}

		_frameCount = 0;
	}

	void Update()
	{
		if (_webCamTexture.didUpdateThisFrame)
		{
			_frameTexture = new Texture2D(_webCamTexture.width, _webCamTexture.height);
			_frameTexture.SetPixels32(_webCamTexture.GetPixels32());
			_frameTexture.Apply();

			SaveFrameToDisk(_frameTexture);
		}
	}

	void SaveFrameToDisk(Texture2D frame)
	{
		byte[] bytes = frame.EncodeToPNG();
		string frameFileName = Path.Combine(_outputFolder, $"frame_{_frameCount:D4}.png");
		File.WriteAllBytes(frameFileName, bytes);
		_frameCount++;
	}

	void OnApplicationQuit()
	{
		// Call FFmpeg to convert the saved frames into a video
		string outputVideoPath = Path.Combine(Application.persistentDataPath, "output_video.mp4").Replace("/", "\\");
		string ffmpegArgs = $"-r 30 -i \"{_outputFolder}\\frame_%04d.png\" -vcodec libx264 -pix_fmt yuv420p \"{outputVideoPath}\"";

		Process ffmpegProcess = new Process();
		ffmpegProcess.StartInfo.FileName = _ffmpegPath;
		ffmpegProcess.StartInfo.Arguments = ffmpegArgs;
		ffmpegProcess.StartInfo.RedirectStandardOutput = true;
		ffmpegProcess.StartInfo.UseShellExecute = false;
		ffmpegProcess.StartInfo.CreateNoWindow = true;
		ffmpegProcess.Start();
		ffmpegProcess.WaitForExit();

		// Clean up the frames after creating the video
		Directory.Delete(_outputFolder, true);
	}
}
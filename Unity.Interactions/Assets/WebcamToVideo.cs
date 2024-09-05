// using System.Diagnostics;
// using System.IO;
// using UnityEngine;
// using Debug = UnityEngine.Debug;
//
// public class WebcamToVideo : MonoBehaviour, IService<IWebcamRecorder>
// {
// 	void Start()
// 	{
// 		var device = WebCamTexture.devices[1];
// 		_webCamTexture = new WebCamTexture(device.name, 640, 480);
// 		_webCamTexture.Play();
//
// 		_outputFolder = Path.Combine(Application.persistentDataPath, "CapturedFrames");
//
// 		if (!Directory.Exists(_outputFolder))
// 			Directory.CreateDirectory(_outputFolder);
//
// 		// Correctly format path to ffmpeg in Plugins directory
// 		_ffmpegPath = Path.Combine(Application.dataPath, "_Project", "Plugins", "ffmpeg.exe").Replace("/", "\\");
//
// 		var ffmpegFile = new FileInfo(_ffmpegPath);
//
// 		// exists?
// 		if (!ffmpegFile.Exists)
// 			Debug.LogError("ffmpeg.exe not found at path: " + _ffmpegPath);
//
// 		_frameCount = 0;
// 	}
//
// 	void Update()
// 	{
// 		if (_webCamTexture.didUpdateThisFrame)
// 		{
// 			_frameTexture = new Texture2D(_webCamTexture.width, _webCamTexture.height);
// 			_frameTexture.SetPixels32(_webCamTexture.GetPixels32());
// 			_frameTexture.Apply();
//
// 			SaveFrameToDisk(_frameTexture);
// 		}
// 	}
//
// 	void OnApplicationQuit()
// 	{
// 		// Call FFmpeg to convert the saved frames into a video
// 		var outputVideoPath = Path.Combine(Application.persistentDataPath, "output_video.mp4").Replace("/", "\\");
// 		// var ffmpegArgsWritingPlainVideo = $"-r 30 -i \"{_outputFolder}\\frame_%04d.png\" -vcodec libx264 -pix_fmt yuv420p \"{outputVideoPath}\"";
// 		string ffmpegArgsWithFrameNumber = $"-r 30 -i \"{_outputFolder}\\frame_%04d.png\" -vf \"drawtext=text='%{{n}}':x=w-tw-10:y=10:fontsize=24:fontcolor=white\" -vcodec libx264 -pix_fmt yuv420p \"{outputVideoPath}\"";
// 		
// 		var ffmpegProcess = new Process();
// 		ffmpegProcess.StartInfo.FileName = _ffmpegPath;
// 		ffmpegProcess.StartInfo.Arguments = ffmpegArgsWithFrameNumber;
// 		ffmpegProcess.StartInfo.RedirectStandardOutput = true;
// 		ffmpegProcess.StartInfo.UseShellExecute = false;
// 		ffmpegProcess.StartInfo.CreateNoWindow = true;
// 		ffmpegProcess.Start();
// 		ffmpegProcess.WaitForExit();
//
// 		// Clean up the frames after creating the video
// 		Directory.Delete(_outputFolder, true);
// 	}
//
// 	void SaveFrameToDisk(Texture2D frame)
// 	{
// 		var bytes = frame.EncodeToPNG();
// 		var frameFileName = Path.Combine(_outputFolder, $"frame_{_frameCount:D4}.png");
// 		File.WriteAllBytes(frameFileName, bytes);
// 		_frameCount++;
// 	}
//
// 	string _ffmpegPath;
// 	int _frameCount;
// 	Texture2D _frameTexture;
// 	string _outputFolder;
// 	WebCamTexture _webCamTexture;
// }
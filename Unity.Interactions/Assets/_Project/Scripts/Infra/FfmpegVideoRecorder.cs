using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace _Project.Scripts.Infra
{
	public class FfmpegVideoRecorder : MonoBehaviour, IWebcamRecorder
	{
		[SerializeField] WebcamSelector _webcamSelector;
		
		public bool IsRecording { get; private set; }

		public void InitiateRecorder()
		{
			if (!TryLocateFFMpeg())
				return;

			var arguments = PrepareArgs();

			_ffmpegProcess = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = _ffmpegPath,
					Arguments = arguments,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					RedirectStandardInput = true,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};

			_ffmpegProcess.ErrorDataReceived += (sender, args) =>
			{
				LogErrors(args);
				WaitForStartOfRecording(args);
			};

			_ffmpegProcess.Start();
			_ffmpegProcess.BeginErrorReadLine();
		}

		string PrepareArgs()
		{
			var webcamName = _webcamSelector.Webcams.First();
			var date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
			var outputPath = Path.Combine(Application.persistentDataPath, "video_" + date + ".mp4").Replace("/", "\\");

			var elapsedTimeExpression = "%{eif\\:t*1000\\:d} ms";
			var drawTextCommand = $"drawtext=text='Elapsed Time\\: {elapsedTimeExpression}':x=10:y=10";
			var arguments = $"-f dshow -i video=\"{webcamName}\" -vcodec libx264 -pix_fmt yuv420p -vf \"{drawTextCommand}\" -y \"{outputPath}\"";

			Debug.Log("FFmpeg arguments: " + arguments);
			return arguments;
		}

		bool TryLocateFFMpeg()
		{
			_ffmpegPath = Path.Combine(Application.dataPath, "_Project", "Plugins", "ffmpeg.exe").Replace("/", "\\");
			var ffmpegFile = new FileInfo(_ffmpegPath);
			if (!ffmpegFile.Exists)
			{
				Debug.LogError("ffmpeg.exe not found at path: " + _ffmpegPath);
				return false;
			}

			return true;
		}

		void WaitForStartOfRecording(DataReceivedEventArgs args)
		{
			if (args.Data.Contains("frame"))
				IsRecording = true;
		}

		static void LogErrors(DataReceivedEventArgs args)
		{
			if (args.Data == null)
				return;
			
			if (!args.Data.ToLower().Contains("error"))
				return;

			if (args.Data.Contains("Fontconfig error: Cannot load default config file"))
				return;
					
			Debug.LogError($"FFmpeg error: {args.Data}");
		}

		public void StopRecording()
		{
			if (_ffmpegProcess == null || _ffmpegProcess.HasExited)
				return;

			_ffmpegProcess.StandardInput.WriteLine("q");
			_ffmpegProcess.StandardInput.Close();

			_ffmpegProcess.WaitForExit();
			IsRecording = false;
			_ffmpegProcess.Dispose();
			_ffmpegProcess = null;
		}


		void OnDestroy()
		{
			StopRecording();
		}
		
		Process _ffmpegProcess;
		string _ffmpegPath;
	}
}
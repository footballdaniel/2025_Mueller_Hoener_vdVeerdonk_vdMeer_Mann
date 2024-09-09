using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace System.Runtime.CompilerServices.VideoRecorder
{
	public class FfmpegVideoRecorder : MonoBehaviour, IWebcamRecorder
	{
		[SerializeField] WebcamSelector _webcamSelector;
		
		Process _ffmpegProcess;
		string _ffmpegPath;
		public bool IsRecording { get; private set; }
		[field: SerializeReference] public float FrameRate { get; private set; }

		public void InitiateRecorder()
		{
			_ffmpegPath = Path.Combine(Application.dataPath, "_Project", "Plugins", "ffmpeg.exe").Replace("/", "\\");

			var ffmpegFile = new FileInfo(_ffmpegPath);

			if (!ffmpegFile.Exists)
			{
				Debug.LogError("ffmpeg.exe not found at path: " + _ffmpegPath);
				return;
			}

			var webcamName = _webcamSelector.Webcams.First();
			var date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
			var outputPath = Path.Combine(Application.persistentDataPath, "video_" + date + ".mp4").Replace("/", "\\");
			
			// var arguments = $"-f dshow -i video=\"{webcamName}\" -vcodec libx264 -pix_fmt yuv420p -y \"{outputPath}\"";
			// var arguments = @$"-f dshow -i video=""{webcamName}"" -vcodec libx264 -pix_fmt yuv420p -vf ""drawtext=text='Elapsed Time: %{{eif:floor(t*10)}}.%{{eif:mod(t*10,10)}} s':fontsize=24:fontcolor=white:x=10:y=10"" -y ""{outputPath}""";
			
			var elapsedTimeExpression = "%{eif\\:t*1000\\:d} ms";
			var drawTextCommand = $"drawtext=text='Elapsed Time\\: {elapsedTimeExpression}':x=10:y=10";
			var arguments = $"-f dshow -i video=\"{webcamName}\" -vcodec libx264 -pix_fmt yuv420p -vf \"{drawTextCommand}\" -y \"{outputPath}\"";



			Debug.Log("FFmpeg arguments: " + arguments);


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
				if (args.Data == null)
					return;
				
				if (args.Data.ToLower().Contains("error"))
					Debug.LogError($"FFmpeg error: {args.Data}");
			
				if (args.Data.Contains("frame")) // Looks for "frame" in the output
				{
					IsRecording = true; // Set IsRecording to true when frame data is found
				}
			};

			_ffmpegProcess.Start();
			_ffmpegProcess.BeginErrorReadLine();
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
	}
}

using System;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;


namespace Interactions.Infra
{
	public static class FFMpegExporter
	{
		public static event Action ExportCompleted;

		public static void EndExport()
		{
			if (_ffmpegProcess == null) return;

			try
			{
				_ffmpegInputStream.Close();
				_ffmpegProcess.WaitForExit();
				_ffmpegProcess.Close();
				ExportCompleted?.Invoke();
			}
			catch (Exception e)
			{
				Debug.LogError($"Error while closing FFmpeg process: {e.Message}");
			}
			finally
			{
				_ffmpegProcess = null;
				_ffmpegInputStream = null;
			}
		}

		public static void StartExport(string videoOutputPath, int width, int height, float frameRate, string ffmpegPath = null)
		{
			ffmpegPath ??= Path.Combine(UnityEngine.Application.streamingAssetsPath, "ffmpeg", "ffmpeg.exe");

			var arguments = $"-f rawvideo -pix_fmt rgba -s {width}x{height} -r {frameRate} -i - -vcodec libx264 -crf 18 -pix_fmt yuv420p -y \"{videoOutputPath}\"";

			_ffmpegProcess = new Process
			{
				StartInfo =
				{
					FileName = ffmpegPath,
					Arguments = arguments,
					UseShellExecute = false,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true
				}
			};

			_ffmpegProcess.OutputDataReceived += (sender, args) => Debug.Log(args.Data);
			_ffmpegProcess.ErrorDataReceived += (sender, args) => Debug.LogError(args.Data);

			_ffmpegProcess.Start();
			_ffmpegInputStream = _ffmpegProcess.StandardInput.BaseStream;

			_ffmpegProcess.BeginOutputReadLine();
			_ffmpegProcess.BeginErrorReadLine();
		}

		public static void WriteFrame(byte[] frameData)
		{
			if (_ffmpegInputStream == null)
				throw new InvalidOperationException("FFmpeg process is not running. Call StartExport() first.");

			_ffmpegInputStream.Write(frameData, 0, frameData.Length);
		}

		static Stream _ffmpegInputStream;

		static Process _ffmpegProcess;
	}
}
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class FFMpegExporter
{
	public static event Action ExportCompleted;

	public static void Export(string inputFramePath, string videoOutputPath, float frameRate, int totalFrames, IProgress<int> progress, string ffmpegPath = null)
	{
		ffmpegPath ??= Path.Combine(Application.streamingAssetsPath, "ffmpeg", "ffmpeg.exe");

		var arguments = $"-r {frameRate} -i \"{inputFramePath}/frame_%06d.png\" -vf \"drawtext=fontfile=/path/to/font.ttf:text='%{{n}}':x=(w-tw)/2:y=h-th-10:fontsize=24:fontcolor=white\" -vcodec libx264 -crf 18 -pix_fmt yuv420p -y \"{videoOutputPath}\"";

		var process = new Process
		{
			StartInfo =
			{
				FileName = ffmpegPath,
				Arguments = arguments,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			}
		};

		process.OutputDataReceived += (sender, args) => ProcessFfmpegOutput(args.Data, totalFrames, progress);
		process.ErrorDataReceived += (sender, args) => ProcessFfmpegOutput(args.Data, totalFrames, progress);
		process.Start();

		process.BeginOutputReadLine();
		process.BeginErrorReadLine();

		process.EnableRaisingEvents = true;
		process.Exited += (sender, args) =>
		{
			progress?.Report(0);
			process.WaitForExit();
			process.Close();
			ExportCompleted?.Invoke();
		};
	}

	static void ProcessFfmpegOutput(string output, int totalFrames, IProgress<int> progress)
	{
		if (string.IsNullOrEmpty(output)) return;

		if (output.Contains("frame="))
		{
			var frameStr = output.Split('=')[1].Trim().Split(' ')[0];

			if (int.TryParse(frameStr, out var currentFrame))
			{
				var progressPercentage = (int)((float)currentFrame / totalFrames * 100);
				progress?.Report(progressPercentage);
			}
		}
	}
}
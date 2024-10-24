using Domain.VideoRecorder;
using UnityEngine;

namespace App
{
	public class InSituViewModel
	{
		readonly IWebcamRecorder _recorder;
		public Texture2D Frame => _recorder?.Frame ?? Texture2D.blackTexture;
		public InSituViewModel(IWebcamRecorder recorder)
		{
			_recorder = recorder;
		}
	}
}
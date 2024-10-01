using Domain.VideoRecorder;
using UnityEngine;

namespace App
{
	internal class WebcamRecorderFactory : IFactory<IWebcamRecorder>
	{
		readonly WebCamConfiguration _selectedRecorder;

		public WebcamRecorderFactory(WebCamConfiguration webCamConfiguration)
		{
			_selectedRecorder = webCamConfiguration;
		}
		
		public IWebcamRecorder Create()
		{
			var recorderGo = new GameObject("WebcamRecorder").AddComponent<FFMpegWebcamRecorder>();
			recorderGo.Set(_selectedRecorder);
			return recorderGo;
		}
	}
}
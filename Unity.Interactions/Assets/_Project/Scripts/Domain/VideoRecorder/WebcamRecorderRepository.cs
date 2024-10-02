using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.WebCam;

namespace Domain.VideoRecorder
{
	public class WebcamRecorderRepository : MonoBehaviour, IRepository<IWebcamRecorder> 
	{
		public IWebcamRecorder Get(int id)
		{
			return _recorders[id];
		}
		

		public IEnumerable<IWebcamRecorder> GetAll()
		{
			var devices = WebCamTexture.devices;
			var uniqueSettings = new HashSet<WebCamConfiguration>();

			foreach (var device in devices)
			foreach (var resolution in VideoCapture.SupportedResolutions)
				uniqueSettings.Add(new WebCamConfiguration(device.name, resolution.width, resolution.height));
			
			ProgressIndicator.Instance.Display("Exporting...", "Frame export", 100);

			_recorders = new List<IWebcamRecorder>();
			foreach (var setting in uniqueSettings)
			{
				var recorder = new FFMpegWebcamRecorder(setting.DeviceName, setting.Width, setting.Height, ProgressIndicator.Instance);
				var go = new GameObject("WebcamRecorder");
				var recorderMonoBehaviour = go.AddComponent<WebCamRecorderMonoBehaviour>();
				recorderMonoBehaviour.Set(recorder);
				_recorders.Add(recorder);
			}
			
			return _recorders;
		}

		List<IWebcamRecorder> _recorders;
	}
}
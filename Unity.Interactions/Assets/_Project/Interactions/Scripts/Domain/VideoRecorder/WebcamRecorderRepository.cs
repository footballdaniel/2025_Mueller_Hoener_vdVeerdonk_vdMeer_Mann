using System.Collections.Generic;
using Interactions.Infra;
using UnityEngine;
using UnityEngine.Windows.WebCam;

namespace Interactions.Domain.VideoRecorder
{
	public class WebcamRecorderRepository : MonoBehaviour, IRepository<IWebcamRecorder>
	{

		void Start()
		{
			var devices = WebCamTexture.devices;
			var uniqueSettings = new HashSet<WebCamConfiguration>();

			foreach (var device in devices)
			foreach (var resolution in VideoCapture.SupportedResolutions)
				uniqueSettings.Add(new WebCamConfiguration(device.name, resolution.width, resolution.height));

			foreach (var setting in uniqueSettings)
			{
				var recorder = new FFMpegWebcamRecorder(setting.DeviceName, setting.Width, setting.Height, ProgressIndicator.Instance);
				_recorders.Add(recorder);
			}
		}

		public IWebcamRecorder Get(int id)
		{
			return _recorders[id];
		}


		public IEnumerable<IWebcamRecorder> GetAll()
		{
			return _recorders;
		}

		public void Add(IWebcamRecorder entity)
		{
			_recorders.Insert(0, entity);
		}

		List<IWebcamRecorder> _recorders = new();
	}
}
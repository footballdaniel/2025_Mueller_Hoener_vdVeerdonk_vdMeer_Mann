using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.WebCam;

namespace Domain.VideoRecorder
{
	public class WebcamRepository : IRepository<WebCamConfiguration>
	{
		public WebCamConfiguration Get(int id)
		{
			return _configs[id];
		}

		public IEnumerable<WebCamConfiguration> GetAll()
		{
			var devices = WebCamTexture.devices;
			var uniqueSettings = new HashSet<WebCamConfiguration>();

			foreach (var device in devices)
			foreach (var resolution in VideoCapture.SupportedResolutions)
				uniqueSettings.Add(new WebCamConfiguration(device.name, resolution.width, resolution.height));

			_configs = new List<WebCamConfiguration>(uniqueSettings);
			return _configs;
		}

		List<WebCamConfiguration> _configs;
	}
}
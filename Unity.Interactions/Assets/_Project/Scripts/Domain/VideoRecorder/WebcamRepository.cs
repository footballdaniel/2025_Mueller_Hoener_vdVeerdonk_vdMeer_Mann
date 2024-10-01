using System.Collections.Generic;
using UnityEngine;

namespace Domain.VideoRecorder
{
	public class WebcamRepository : IRepository<WebCamConfiguration>
	{
		public WebcamRepository()
		{
		}

		public WebCamConfiguration Get(int id)
		{
			return _configs[id];
		}

		public IEnumerable<WebCamConfiguration> GetAll()
		{
			var devices = WebCamTexture.devices;

			_configs = new List<WebCamConfiguration>();

			foreach (var device in devices)
			{
				var config = new WebCamConfiguration(device.name, 1920, 1080, 30);
				_configs.Add(config);
			}
			
			return _configs;
		}

		List<WebCamConfiguration> _configs;
	}
}
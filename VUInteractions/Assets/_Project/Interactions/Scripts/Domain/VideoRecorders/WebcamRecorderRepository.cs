using System.Collections.Generic;
using Interactions.Infra;
using UnityEngine;
using UnityEngine.Windows.WebCam;

namespace Interactions.Domain.VideoRecorders
{
	public class WebcamRecorderRepository : MonoBehaviour, IRepository<IWebcamRecorder>
	{
		void Start()
		{
			var devices = WebCamTexture.devices;
			var uniqueSettings = new HashSet<WebcamSpecs>();

			foreach (var device in devices)
			foreach (var resolution in VideoCapture.SupportedResolutions)
			{
				var frameRates = VideoCapture.GetSupportedFrameRatesForResolution(resolution);
				var minFrameRate = float.MaxValue;

				foreach (var frameRate in frameRates)
					if (frameRate < minFrameRate)
						minFrameRate = frameRate;

				uniqueSettings.Add(new WebcamSpecs(device.name, resolution.width, resolution.height, (int)minFrameRate));
			}

			foreach (var specs in uniqueSettings)
				_recorders.Add(new FfMpegWebcamRecorder(specs));
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
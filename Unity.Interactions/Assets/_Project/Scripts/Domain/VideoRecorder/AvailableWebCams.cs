using System.Collections.Generic;
using UnityEngine;

public record WebCamConfiguration(string DeviceName, int Width, int Height, int FrameRate);

public class AvailableWebCams : List<WebCamConfiguration>
{
	public AvailableWebCams()
	{
		var devices = WebCamTexture.devices;
		foreach (var device in devices)
		{
			var config = new WebCamConfiguration(device.name, 1920, 1080, 30);
			Add(config);
		}
	}
}
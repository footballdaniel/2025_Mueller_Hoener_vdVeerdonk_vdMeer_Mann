using System.Collections.Generic;
using UnityEngine;

namespace _Project.Interactions.Scripts.Domain.VideoRecorder
{
	public class WebcamSelector : MonoBehaviour
	{
		[field: SerializeReference] public List<string> Webcams { get; private set; }
	
		void OnValidate()
		{
			Webcams = new List<string>();

			var devices = WebCamTexture.devices;

			foreach (var device in devices)
				Webcams.Add(device.name);
		}
	}
}
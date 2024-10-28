using System;
using UnityEngine.XR.Management;

namespace App
{
	internal class XRStatus
	{
		public static event Action XRStartupError;

		public static bool HasXRErrors()
		{
			var initialized = XRGeneralSettings.Instance.Manager.isInitializationComplete;

			if (!initialized)
				XRStartupError?.Invoke();
			
			return !initialized;
		}
	}
}
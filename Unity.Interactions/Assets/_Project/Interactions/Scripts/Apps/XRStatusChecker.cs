using UnityEngine.XR.Management;

namespace Interactions.Apps
{
	internal static class XRStatusChecker
	{
		public static bool HasXRErrors()
		{
			var initialized = XRGeneralSettings.Instance.Manager.isInitializationComplete;
			return !initialized;
		}
	}
}
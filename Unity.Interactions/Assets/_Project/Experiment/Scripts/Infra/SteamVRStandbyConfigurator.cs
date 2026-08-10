using UnityEngine;

namespace Interactions.Infra
{
	public static class SteamVRStandbyConfigurator
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void DisableStandbyBeforeXRStarts()
		{
			if (SteamVRHeadlessConfig.IsTrackingKeptWithHeadsetOff())
				return;

			if (SteamVRHeadlessConfig.IsSteamVrRunning())
			{
				Debug.LogWarning(
					"SteamVR is already running, so it cannot be stopped from pausing tracking when the " +
					"headset is off. Close SteamVR and enter play mode again.");
				return;
			}

			if (SteamVRHeadlessConfig.KeepTrackingWithHeadsetOff())
				Debug.Log("SteamVR will keep tracking while the headset is off.");
		}
	}
}

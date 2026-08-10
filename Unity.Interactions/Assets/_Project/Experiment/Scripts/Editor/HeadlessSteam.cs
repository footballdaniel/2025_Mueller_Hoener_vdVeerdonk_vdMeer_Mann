using Interactions.Infra;
using UnityEditor;
using UnityEngine;

namespace _Project.Interactions.Editor
{
	public static class SteamVRConfigModifier
	{
		[MenuItem("STEAMVR/Use Trackers Without Headset")]
		public static void UseTrackersWithoutHeadset()
		{
			Report(SteamVRHeadlessConfig.Apply(headless: true), "headless tracker use");
		}

		[MenuItem("STEAMVR/Use Headset")]
		public static void UseHeadset()
		{
			Report(SteamVRHeadlessConfig.Apply(headless: false), "headset use");
		}

		static void Report(bool applied, string mode)
		{
			if (applied)
				Debug.Log($"SteamVR is now configured for {mode}.");
			else
				Debug.LogError($"Could not configure SteamVR for {mode}. {SteamVRHeadlessConfig.LastError}");
		}
	}
}

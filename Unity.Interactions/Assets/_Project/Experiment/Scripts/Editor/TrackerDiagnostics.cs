using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

namespace _Project.Interactions.Editor
{
	public static class TrackerDiagnostics
	{
		[MenuItem("STEAMVR/Log Tracker State")]
		public static void LogTrackerState()
		{
			var report = new StringBuilder();

			report.AppendLine($"Play mode: {Application.isPlaying}");
			report.AppendLine();
			report.AppendLine("--- Input System devices ---");

			foreach (var device in InputSystem.devices)
				report.AppendLine(
					$"{device.layout} \"{device.name}\" usages=[{string.Join(",", device.usages)}] " +
					$"added={device.added} enabled={device.enabled}");

			report.AppendLine();
			report.AppendLine("--- XRTracker components ---");

			var trackers = Object.FindObjectsByType<XRTracker>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			if (trackers.Length == 0)
				report.AppendLine("none found (enter play mode first)");

			foreach (var tracker in trackers)
				report.AppendLine(Describe(tracker));

			Debug.Log(report.ToString());
		}

		static string Describe(XRTracker tracker)
		{
			var serialized = new SerializedObject(tracker);
			var reference = serialized.FindProperty("_positionAction")?.objectReferenceValue as InputActionReference;
			var action = reference != null ? reference.action : null;

			if (action == null)
				return $"[{tracker.TrackerMappingName}] no InputActionReference assigned";

			var text = new StringBuilder();
			text.Append($"[{tracker.TrackerMappingName}] action=\"{action.name}\" enabled={action.enabled} " +
			            $"resolvedControls={action.controls.Count}");

			foreach (var binding in action.bindings)
				text.Append($"\n    binding: {binding.path}");

			foreach (var control in action.controls)
			{
				var device = control.device;
				var tracked = device as TrackedDevice;
				text.Append($"\n    -> {control.path} on {device.layout} " +
				            $"usages=[{string.Join(",", device.usages)}] " +
				            $"isTracked={(tracked != null ? tracked.isTracked.isPressed.ToString() : "not a TrackedDevice")}");
			}

			text.Append($"\n    IsTracked={tracker.IsTracked} Position={tracker.Position}");
			return text.ToString();
		}
	}
}

using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;
using Debug = UnityEngine.Debug;

namespace Interactions.Infra
{
	public static class SteamVRHeadlessConfig
	{
		public static string LastError { get; private set; }

		public static bool Apply(bool headless)
		{
			LastError = null;

			if (IsSteamVrRunning())
			{
				LastError = "SteamVR is running. Close it first, otherwise it overwrites steamvr.vrsettings on exit.";
				Debug.LogWarning(LastError);
				return false;
			}

			try
			{
				PatchUserSettings(headless);
				PatchSteamVrDefaults(headless);
				PatchNullDriverDefaults(headless);
				return true;
			}
			catch (Exception exception)
			{
				LastError = exception.Message;
				Debug.LogError($"Could not configure SteamVR: {exception}");
				return false;
			}
		}

		public static bool KeepTrackingWithHeadsetOff()
		{
			LastError = null;

			if (IsSteamVrRunning())
			{
				LastError = "SteamVR is running. Close it first, otherwise it overwrites steamvr.vrsettings on exit.";
				Debug.LogWarning(LastError);
				return false;
			}

			try
			{
				var configFile = UserSettings();
				JObject json;

				if (configFile.Exists)
					json = JObject.Parse(File.ReadAllText(configFile.FullName));
				else
				{
					configFile.Directory?.Create();
					json = new JObject();
				}

				var power = Section(json, "power");
				power["pauseCompositorOnStandby"] = false;
				power["turnOffControllersTimeout"] = KeepAwakeSeconds;
				power["powerOffOnExit"] = false;

				File.WriteAllText(configFile.FullName, json.ToString());
				Debug.Log($"Disabled SteamVR standby in {configFile.FullName}.");
				return true;
			}
			catch (Exception exception)
			{
				LastError = exception.Message;
				Debug.LogError($"Could not change the SteamVR standby behaviour: {exception}");
				return false;
			}
		}

		public static bool IsTrackingKeptWithHeadsetOff()
		{
			try
			{
				var pausesOnStandby = ReadBool(UserSettings(), "power", "pauseCompositorOnStandby");
				var powersOffOnExit = ReadBool(UserSettings(), "power", "powerOffOnExit");
				var controllersTimeout = ReadToken(UserSettings(), "power", "turnOffControllersTimeout")
					?.Value<float>();

				return pausesOnStandby == false && powersOffOnExit == false &&
				       controllersTimeout >= KeepAwakeSeconds;
			}
			catch (Exception exception)
			{
				Debug.LogWarning($"Could not read the SteamVR standby configuration: {exception.Message}");
				return false;
			}
		}

		public static bool IsHeadlessConfigured()
		{
			try
			{
				var requireHmd = ReadBool(UserSettings(), "steamvr", "requireHmd")
				                 ?? ReadBool(SteamVrDefaults(), "steamvr", "requireHmd");
				var forcedDriver = ReadString(UserSettings(), "steamvr", "forcedDriver")
				                   ?? ReadString(SteamVrDefaults(), "steamvr", "forcedDriver");
				var nullDriverEnabled = ReadBool(UserSettings(), "driver_null", "enable")
				                        ?? ReadBool(NullDriverDefaults(), "driver_null", "enable");

				return requireHmd == false && forcedDriver == "null" && nullDriverEnabled == true;
			}
			catch (Exception exception)
			{
				Debug.LogWarning($"Could not read the SteamVR configuration: {exception.Message}");
				return false;
			}
		}

		public static bool IsSteamVrRunning()
		{
			return Process.GetProcessesByName("vrserver").Length > 0 ||
			       Process.GetProcessesByName("vrmonitor").Length > 0;
		}

		public static bool LaunchSteamVr()
		{
			LastError = null;

			var startup = new FileInfo(Path.Combine(SteamVrRoot, "bin", "win64", "vrstartup.exe"));
			if (!startup.Exists)
			{
				LastError = $"vrstartup.exe not found at {startup.FullName}.";
				Debug.LogError(LastError);
				return false;
			}

			try
			{
				Process.Start(new ProcessStartInfo(startup.FullName)
				{
					UseShellExecute = true,
					WorkingDirectory = startup.DirectoryName
				});
				return true;
			}
			catch (Exception exception)
			{
				LastError = exception.Message;
				Debug.LogError($"Could not start SteamVR: {exception}");
				return false;
			}
		}

		#region Implementation

		const int KeepAwakeSeconds = 86400;

		static string SteamVrRoot => Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
			"Steam", "steamapps", "common", "SteamVR");

		static FileInfo SteamVrDefaults() =>
			new(Path.Combine(SteamVrRoot, "resources", "settings", "default.vrsettings"));

		static FileInfo NullDriverDefaults() =>
			new(Path.Combine(SteamVrRoot, "drivers", "null", "resources", "settings", "default.vrsettings"));

		static FileInfo UserSettings() =>
			new(Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
				"Steam", "config", "steamvr.vrsettings"));

		static void PatchUserSettings(bool headless)
		{
			var configFile = UserSettings();
			JObject json;

			if (configFile.Exists)
				json = JObject.Parse(File.ReadAllText(configFile.FullName));
			else
			{
				configFile.Directory?.Create();
				json = new JObject();
			}

			var steamvr = Section(json, "steamvr");
			steamvr["requireHmd"] = !headless;
			steamvr["forcedDriver"] = headless ? "null" : "";
			steamvr["activateMultipleDrivers"] = headless;

			Section(json, "driver_null")["enable"] = headless;

			File.WriteAllText(configFile.FullName, json.ToString());
			Debug.Log($"{(headless ? "Configured" : "Restored")} {configFile.FullName}.");
		}

		static void PatchSteamVrDefaults(bool headless)
		{
			var configFile = SteamVrDefaults();
			if (!configFile.Exists)
			{
				Debug.LogWarning($"{configFile.FullName} not found; relying on the per-user overrides.");
				return;
			}

			var json = JObject.Parse(File.ReadAllText(configFile.FullName));
			var steamvr = Section(json, "steamvr");
			steamvr["requireHmd"] = !headless;
			steamvr["forcedDriver"] = headless ? "null" : "";
			steamvr["activateMultipleDrivers"] = headless;

			File.WriteAllText(configFile.FullName, json.ToString());
		}

		static void PatchNullDriverDefaults(bool headless)
		{
			var configFile = NullDriverDefaults();
			if (!configFile.Exists)
			{
				Debug.LogWarning($"{configFile.FullName} not found; relying on the per-user overrides.");
				return;
			}

			var json = JObject.Parse(File.ReadAllText(configFile.FullName));
			Section(json, "driver_null")["enable"] = headless;

			File.WriteAllText(configFile.FullName, json.ToString());
		}

		static JObject Section(JObject root, string name)
		{
			if (root[name] is JObject existing)
				return existing;

			var created = new JObject();
			root[name] = created;
			return created;
		}

		static bool? ReadBool(FileInfo file, string section, string key) =>
			ReadToken(file, section, key)?.Value<bool>();

		static string ReadString(FileInfo file, string section, string key) =>
			ReadToken(file, section, key)?.Value<string>();

		static JToken ReadToken(FileInfo file, string section, string key)
		{
			if (!file.Exists)
				return null;

			var json = JObject.Parse(File.ReadAllText(file.FullName));
			return (json[section] as JObject)?[key];
		}

		#endregion
	}
}

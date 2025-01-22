using System;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace _Project.Interactions.Editor
{
	public static class SteamVRConfigModifier
	{
		[MenuItem("STEAMVR/Use Trackers Without Headset")]
		public static void UseTrackersWithoutHeadset()
		{
			if (SteamVRConfig().Exists)
				ModifySteamVrConfigFile();
			else
				Debug.LogError("default.vrsettings file not found.");

			if (NullDriver().Exists)
				ModifyNullDriverConfig();
			else
				Debug.LogError("null driver file not found.");
		}

		[MenuItem("STEAMVR/Use Headset")]
		public static void UseHeadset()
		{
			if (SteamVRConfig().Exists)
				UndoModifySteamVrConfigFile();
			else
				Debug.LogError("default.vrsettings file not found.");


			if (NullDriver().Exists)
				UndoModifyNullConfig();
			else
				Debug.LogError("null driver file not found.");
		}

		#region Implementation

		static FileInfo SteamVRConfig()
		{
			var steamVrConfig = new FileInfo(Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
				"Steam",
				"steamapps",
				"common",
				"SteamVR",
				"resources",
				"settings",
				"default.vrsettings"
			));
			return steamVrConfig;
		}

		static FileInfo NullDriver()
		{
			var nullDriver = new FileInfo(Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
				"Steam",
				"steamapps",
				"common",
				"SteamVR",
				"drivers",
				"null",
				"resources",
				"settings",
				"default.vrsettings"
			));
			return nullDriver;
		}


		static void UndoModifySteamVrConfigFile()
		{
			var configFile = SteamVRConfig();
			var jsonContent = File.ReadAllText(configFile.FullName);
			var json = JObject.Parse(jsonContent);

			if (json["steamvr"] != null)
			{
				json["steamvr"]["requireHmd"] = true;
				json["steamvr"]["forcedDriver"] = "";
				json["steamvr"]["activateMultipleDrivers"] = false;
			}
			else
			{
				Debug.LogError("No 'steamvr' section found in default.vrsettings.");
				return;
			}

			File.WriteAllText(configFile.FullName, json.ToString());
			Debug.Log("Restored default.vrsettings to original settings.");
		}


		static void ModifySteamVrConfigFile()
		{
			var configFile = SteamVRConfig();
			var jsonContent = File.ReadAllText(configFile.FullName);
			var json = JObject.Parse(jsonContent);

			if (json["steamvr"] != null)
			{
				json["steamvr"]["requireHmd"] = false;
				json["steamvr"]["forcedDriver"] = "null";
				json["steamvr"]["activateMultipleDrivers"] = true;
			}
			else
			{
				Debug.LogError("No 'steamvr' section found in default.vrsettings.");
				return;
			}

			File.WriteAllText(configFile.FullName, json.ToString());
			Debug.Log("Modified default.vrsettings successfully.");
		}

		static void ModifyNullDriverConfig()
		{
			var configFile = NullDriver();
			var jsonContent = File.ReadAllText(configFile.FullName);
			var json = JObject.Parse(jsonContent);

			if (json["driver_null"] != null)
				json["driver_null"]["enable"] = true;
			else
			{
				Debug.LogError("No 'driver_null' section found in null driver default.vrsettings.");
				return;
			}

			File.WriteAllText(configFile.FullName, json.ToString());
			Debug.Log("Modified null driver default.vrsettings successfully.");
		}

		static void UndoModifyNullConfig()
		{
			var configFile = NullDriver();
			var jsonContent = File.ReadAllText(configFile.FullName);
			var json = JObject.Parse(jsonContent);

			if (json["driver_null"] != null)
				json["driver_null"]["enable"] = false;
			else
			{
				Debug.LogError("No 'driver_null' section found in null driver default.vrsettings.");
				return;
			}

			File.WriteAllText(configFile.FullName, json.ToString());
			Debug.Log("Restored null driver default.vrsettings to original settings.");
		}

		#endregion

	}
}
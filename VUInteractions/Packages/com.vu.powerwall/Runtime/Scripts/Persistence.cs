using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

internal class Persistence
{

	class Vector3QuaternionConverter : JsonConverter
	{

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Vector3) || objectType == typeof(Quaternion);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jsonObject = JObject.Load(reader);

			if (objectType == typeof(Vector3))
			{
				return new Vector3(
					(float)jsonObject["x"],
					(float)jsonObject["y"],
					(float)jsonObject["z"]
				);
			}

			if (objectType == typeof(Quaternion))
			{
				return new Quaternion(
					(float)jsonObject["x"],
					(float)jsonObject["y"],
					(float)jsonObject["z"],
					(float)jsonObject["w"]
				);
			}

			throw new NotSupportedException($"Cannot convert object of type {objectType}");
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var jsonObject = new JObject();

			if (value is Vector3 v3)
			{
				jsonObject["x"] = v3.x;
				jsonObject["y"] = v3.y;
				jsonObject["z"] = v3.z;
			}
			else if (value is Quaternion q)
			{
				jsonObject["x"] = q.x;
				jsonObject["y"] = q.y;
				jsonObject["z"] = q.z;
				jsonObject["w"] = q.w;
			}
			else
				throw new NotSupportedException($"Cannot convert object of type {value.GetType()}");

			jsonObject.WriteTo(writer);
		}
	}

	public void Save(CalibrationData calibrationData)
	{
		var json = JsonConvert.SerializeObject(calibrationData, new Vector3QuaternionConverter());
		File.WriteAllText("calibration.json", json);
	}

	public CalibrationData TryLoadCalibration()
	{
		if (File.Exists("calibration.json"))
		{
			var json = File.ReadAllText("calibration.json");
			var deserialized = JsonConvert.DeserializeObject<CalibrationData>(json, new Vector3QuaternionConverter());

			if (deserialized != null)
				return deserialized;
		}

		return new CalibrationData(Vector3.zero, Quaternion.identity);
	}
}
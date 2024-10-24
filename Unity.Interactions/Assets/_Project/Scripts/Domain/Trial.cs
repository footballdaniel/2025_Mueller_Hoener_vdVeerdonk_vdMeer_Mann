using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace Domain
{
	public class Trial
	{
		public Trial(float startTime)
		{
			_timestamps = new List<float>();
			StartTime = startTime;
		}

		public void Tick(float deltaTime)
		{
			Duration += deltaTime;
			_timestamps.Add(Duration);
		}

		public void Save()
		{
			var jsonSettings = new JsonSerializerSettings();
			jsonSettings.Converters.Add(new Vector3Converter());

			var jsonData = JsonConvert.SerializeObject(this, jsonSettings);
			var path = Path.Combine(Application.persistentDataPath, "trial_data.json");
			File.WriteAllText(path, jsonData);
		}

		readonly List<float> _timestamps;
		public List<Vector3> OpponentHipPositions { get; } = new();

		public float StartTime { get; }
		public float Duration { get; private set; }
	}

	public class Vector3Converter : JsonConverter<Vector3>
	{
		public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("x");
			writer.WriteValue(value.x);
			writer.WritePropertyName("y");
			writer.WriteValue(value.y);
			writer.WritePropertyName("z");
			writer.WriteValue(value.z);
			writer.WriteEndObject();
		}

		public override Vector3 ReadJson(JsonReader reader, System.Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			float x = 0, y = 0, z = 0;

			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.PropertyName)
				{
					var propertyName = (string)reader.Value;
					reader.Read();
					switch (propertyName)
					{
						case "x": x = (float)(double)reader.Value; break;
						case "y": y = (float)(double)reader.Value; break;
						case "z": z = (float)(double)reader.Value; break;
					}
				}
				else if (reader.TokenType == JsonToken.EndObject)
				{
					break;
				}
			}

			return new Vector3(x, y, z);
		}
	}
}
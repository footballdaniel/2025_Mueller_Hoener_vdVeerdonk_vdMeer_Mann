using System;
using System.Collections.Generic;
using System.Linq;
using _Project.PassDetection.Validation;
using Newtonsoft.Json;
using Unity.Sentis;

namespace PassDetection.Validation
{

	public class SplitEnumConverter : JsonConverter<Split>
	{

		public override Split ReadJson(JsonReader reader, Type objectType, Split existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			// Deserialize the string back to the enum
			if (reader.TokenType == JsonToken.String)
			{
				var enumText = reader.Value?.ToString();

				if (Enum.TryParse(enumText, true, out Split result))
					return result;
			}

			throw new JsonSerializationException($"Error converting value {reader.Value} to enum {nameof(Split)}");
		}

		public override void WriteJson(JsonWriter writer, Split value, JsonSerializer serializer)
		{
			// Serialize the enum as its name (e.g., "TRAIN")
			writer.WriteValue(value.ToString());
		}
	}
}
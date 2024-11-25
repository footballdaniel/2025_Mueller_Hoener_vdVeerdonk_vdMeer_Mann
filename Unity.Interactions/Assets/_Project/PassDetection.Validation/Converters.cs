using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Unity.Sentis;

namespace _Project.PassDetection.Validation
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
	
	
	
	public static class InferenceExtensions
	{
		public static Tensor ToTensor(this Inference inference)
		{
			if (inference.Targets.Count != 12 || inference.Targets.Any(f => f.Values.Count != 10))
				throw new ArgumentException("Targets must contain exactly 12 items, each with 10 values.");
			
			var tensor = new Tensor<float>(new TensorShape(1, 10, 12));

			for (var i = 0; i < 12; i++)
			for (var j = 0; j < 10; j++)
				tensor[0, j, i] = inference.Targets[i].Values[j];


			return tensor;
		}
	}
}
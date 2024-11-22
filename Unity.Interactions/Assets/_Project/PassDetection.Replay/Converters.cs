using System;
using Newtonsoft.Json;

namespace _Project.PassDetection.Replay
{
	public class RoundedFloatConverter : JsonConverter<float>
	{
		private readonly int _decimalPlaces;

		public RoundedFloatConverter(int decimalPlaces)
		{
			_decimalPlaces = decimalPlaces;
		}

		public override void WriteJson(JsonWriter writer, float value, JsonSerializer serializer)
		{
			writer.WriteValue(Math.Round(value, _decimalPlaces));
		}

		public override float ReadJson(JsonReader reader, Type objectType, float existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return (float)reader.Value;
		}
	}
}
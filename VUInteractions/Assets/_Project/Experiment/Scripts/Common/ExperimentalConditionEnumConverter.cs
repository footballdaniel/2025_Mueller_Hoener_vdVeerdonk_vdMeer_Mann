using System;
using Interactions.Apps;
using Newtonsoft.Json;

namespace Interactions.Domain
{
	public class ExperimentalConditionEnumConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(((ExperimentalCondition)value).ToString());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return Enum.Parse(typeof(ExperimentalCondition), (string)reader.Value);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(ExperimentalCondition);
		}
	}
}
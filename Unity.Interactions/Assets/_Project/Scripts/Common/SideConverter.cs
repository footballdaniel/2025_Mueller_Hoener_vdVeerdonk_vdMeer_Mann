using System;
using DefaultNamespace;
using Newtonsoft.Json;

public class SideEnumConverter : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		var side = (Side)value;
		writer.WriteValue(side == Side.RIGHT ? "Right" : "Left");
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		var sideValue = reader.Value.ToString();
		return sideValue.Equals("Right", StringComparison.OrdinalIgnoreCase) ? Side.RIGHT : Side.LEFT;
	}

	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Side);
	}
}

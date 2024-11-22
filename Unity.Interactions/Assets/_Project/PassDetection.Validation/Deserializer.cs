using System.Collections.Generic;
using System.IO;
using _Project.PassDetection.Validation;
using Newtonsoft.Json;

public class SampleDeserializer
{
	public static IEnumerable<Sample> DeserializeSamples(string filePath)
	{
		using (var fileStream = File.OpenText(filePath))
		using (var reader = new JsonTextReader(fileStream))
		{
			var serializer = new JsonSerializer();

			if (reader.Read() && reader.TokenType == JsonToken.StartArray)
			{
				while (reader.Read() && reader.TokenType == JsonToken.StartObject)
					yield return serializer.Deserialize<Sample>(reader);
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gamepedia.Lol.Api.Converters
{
	internal class InternalJsonConverter<T> : JsonConverter<T> where T : class, new()
	{
		public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			// Handle null values
			if (reader.TokenType == JsonTokenType.Null)
			{
				return null;
			}

			using var jsonDocument = JsonDocument.ParseValue(ref reader);
			var jsonRootText = jsonDocument.RootElement.GetRawText();
			var jsonValuePairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonRootText);

			var obj = new T();
			var objType = obj.GetType();

			// Include all properties that matches the given binding attributes except properties that have the 'JsonIgnoreAttribute'
			var objProperties = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => !x.CustomAttributes.Any(attr => attr.AttributeType == typeof(JsonIgnoreAttribute)));

			// Get all private fields that are not auto generated backing fields
			var objFields = objType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(x => !x.Name.EndsWith("k__BackingField"));

			foreach (var jsonPropertyName in jsonValuePairs?.Keys ?? Enumerable.Empty<string>())
			{
				object? propertyValue = null;
				var normalizedJsonPropertyName = jsonPropertyName.Replace(" ", "");
				
				foreach(var property in objProperties)
				{
					if (property.Name.Replace("_", "") == normalizedJsonPropertyName)
					{
						if (jsonValuePairs!.TryGetValue(jsonPropertyName, out var value))
						{
							propertyValue = objType.InvokeMember(
								property.Name,
								BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance,
								null,
								obj,
								new[] { JsonSerializer.Deserialize(value.GetRawText(), property.PropertyType) });
							break;
						}
					}
				}

				// TODO: Check if t = new() is fixed in Preview 2
				if (propertyValue is null)
				{
					foreach (var field in objFields)
					{
						if (field.Name.Equals(normalizedJsonPropertyName, StringComparison.OrdinalIgnoreCase))
						{
							if (jsonValuePairs!.TryGetValue(jsonPropertyName, out var value))
							{
								field.SetValue(obj, value.ToString());
								break;
							}
						}
					}
				}
			}

			return obj;
		}

		public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}
}
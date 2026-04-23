#nullable enable
using Newtonsoft.Json;
using System;
using ADTOSharp.Extensions;

namespace ADTO.DCloud.Infrastructure
{
    public class TimeSpanToJsonStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value != null)
            {
                writer.WriteValue(value.ToString());
            }
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return default;
            }

            var str = (string)reader.Value;

            if (str.IsNullOrWhiteSpace())
            {
                return default;
            }

            return TimeSpan.Parse(str);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(TimeSpan).IsAssignableFrom(objectType) || typeof(TimeSpan?).IsAssignableFrom(objectType);
        }
    }
}

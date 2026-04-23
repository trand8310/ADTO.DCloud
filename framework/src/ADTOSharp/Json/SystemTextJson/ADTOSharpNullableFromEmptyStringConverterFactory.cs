using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using ADTOSharp.Extensions;

namespace ADTOSharp.Json.SystemTextJson
{
    public class ADTOSharpNullableFromEmptyStringConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.GetTypeInfo().IsGenericType &&
                   typeToConvert.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(
                typeof(ADTOSharpNullableFromEmptyStringConverter<>).MakeGenericType(typeToConvert),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                null,
                culture: null);
        }
    }

    public class ADTOSharpNullableFromEmptyStringConverter<TNullableType> : JsonConverter<TNullableType>
    {
        private JsonSerializerOptions _readJsonSerializerOptions;
        private JsonSerializerOptions _writeJsonSerializerOptions;

        public override TNullableType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (_readJsonSerializerOptions == null)
            {
                _readJsonSerializerOptions = JsonSerializerOptionsHelper.Create(options, x =>
                    x == this ||
                    x.GetType() == typeof(ADTOSharpNullableFromEmptyStringConverterFactory));
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                if (reader.GetString().IsNullOrWhiteSpace())
                {
                    return default;
                }
            }

            return JsonSerializer.Deserialize<TNullableType>(ref reader, _readJsonSerializerOptions);
        }

        public override void Write(Utf8JsonWriter writer, TNullableType value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            if (_writeJsonSerializerOptions == null)
            {
                _writeJsonSerializerOptions = JsonSerializerOptionsHelper.Create(options, x =>
                    x == this ||
                    x.GetType() == typeof(ADTOSharpNullableFromEmptyStringConverterFactory));
            }

            var converter = (JsonConverter<TNullableType>) _writeJsonSerializerOptions.GetConverter(typeof(TNullableType));
            converter.Write(writer, value, _writeJsonSerializerOptions);
        }
    }
}

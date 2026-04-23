using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ADTOSharp.Extensions;
using ADTOSharp.Reflection;

namespace ADTOSharp.Json.SystemTextJson
{
    public class ADTOSharpJsonConverterForType: JsonConverter<Type>
    {
        public override Type Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var assemblyQualifiedName = reader.GetString();
            if (assemblyQualifiedName.IsNullOrEmpty())
            {
                throw new Exception("AssemblyQualifiedName is empty!");
            }
            
            return Type.GetType(assemblyQualifiedName);
        }

        public override void Write(
            Utf8JsonWriter writer,
            Type value,
            JsonSerializerOptions options
        )
        {
            var assemblyQualifiedName = TypeHelper.SerializeType(value).ToString();
            writer.WriteStringValue(assemblyQualifiedName);
        }
    }
}
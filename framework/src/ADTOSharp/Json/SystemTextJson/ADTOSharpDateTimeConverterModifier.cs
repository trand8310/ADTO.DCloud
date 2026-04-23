using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ADTOSharp.Reflection;
using ADTOSharp.Timing;

namespace ADTOSharp.Json.SystemTextJson
{
    public class ADTOSharpDateTimeConverterModifier
    {
        private readonly List<string> _inputDateTimeFormats;
        private readonly string _outputDateTimeFormat;

        public ADTOSharpDateTimeConverterModifier(List<string> inputDateTimeFormats, string outputDateTimeFormat)
        {
            _inputDateTimeFormats = inputDateTimeFormats;
            _outputDateTimeFormat = outputDateTimeFormat;
        }

        public Action<JsonTypeInfo> CreateModifyAction()
        {
            return Modify;
        }

        private void Modify(JsonTypeInfo jsonTypeInfo)
        {
            if (ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableDateTimeNormalizationAttribute>(jsonTypeInfo.Type) != null)
            {
                return;
            }

            foreach (var property in jsonTypeInfo.Properties.Where(x => x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?)))
            {
                if (property.AttributeProvider == null ||
                    !property.AttributeProvider.GetCustomAttributes(typeof(DisableDateTimeNormalizationAttribute), false).Any())
                {
                    property.CustomConverter = property.PropertyType == typeof(DateTime)
                        ? (JsonConverter) new ADTOSharpDateTimeConverter(_inputDateTimeFormats, _outputDateTimeFormat)
                        : new ADTOSharpNullableDateTimeConverter(_inputDateTimeFormats, _outputDateTimeFormat);
                }
            }
        }
    }
}

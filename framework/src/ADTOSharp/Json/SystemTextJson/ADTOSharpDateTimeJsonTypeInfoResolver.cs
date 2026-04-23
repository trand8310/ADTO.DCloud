using System.Collections.Generic;
using System.Text.Json.Serialization.Metadata;

namespace ADTOSharp.Json.SystemTextJson
{
    public class ADTOSharpDateTimeJsonTypeInfoResolver : DefaultJsonTypeInfoResolver
    {
        public ADTOSharpDateTimeJsonTypeInfoResolver(List<string> inputDateTimeFormats = null, string outputDateTimeFormat = null)
        {
            Modifiers.Add(new ADTOSharpDateTimeConverterModifier(inputDateTimeFormats, outputDateTimeFormat).CreateModifyAction());
        }
    }
}

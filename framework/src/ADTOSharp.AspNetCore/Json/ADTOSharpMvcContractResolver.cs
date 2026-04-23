using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ADTOSharp.Json;

public class ADTOSharpMvcContractResolver : DefaultContractResolver
{
    private List<string> InputDateTimeFormats { get; set; }
    private string OutputDateTimeFormat { get; set; }

    public ADTOSharpMvcContractResolver(List<string> inputDateTimeFormats = null, string outputDateTimeFormat = null)
    {
        InputDateTimeFormats = inputDateTimeFormats ?? new List<string>();
        OutputDateTimeFormat = outputDateTimeFormat;
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        ModifyProperty(member, property);

        return property;
    }

    protected virtual void ModifyProperty(MemberInfo member, JsonProperty property)
    {
        if (!ADTOSharpDateTimeConverter.ShouldNormalize(member, property))
        {
            return;
        }

        property.Converter = new ADTOSharpDateTimeConverter(InputDateTimeFormats, OutputDateTimeFormat);
    }
}
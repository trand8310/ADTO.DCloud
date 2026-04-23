using System;
using System.Reflection;
using ADTOSharp.Reflection;
using ADTOSharp.Timing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ADTOSharp.Json
{
    public class ADTOSharpContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            ModifyProperty(member, property);

            return property;
        }

        protected virtual void ModifyProperty(MemberInfo member, JsonProperty property)
        {
            if (ADTOSharpDateTimeConverter.ShouldNormalize(member, property))
            {
                property.Converter = new ADTOSharpDateTimeConverter();
            }
        }
    }
}

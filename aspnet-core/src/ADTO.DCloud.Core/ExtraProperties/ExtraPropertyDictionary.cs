using System;
using System.Collections.Generic;
using ADTOSharp.Json;

namespace ADTO.DCloud.ExtraProperties;

[Serializable]
public class ExtraPropertyDictionary : Dictionary<string, object>
{
    public ExtraPropertyDictionary()
    {
    }

    public ExtraPropertyDictionary(IDictionary<string, object> dictionary)
        : base(dictionary)
    {
    }

    public override string ToString()
    {
        return this.ToJsonString();
    }
}
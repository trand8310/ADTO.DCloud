using ADTOSharp.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ADTOSharp.Reflection;

namespace ADTOSharp.Runtime.Caching
{
    /// <summary>
    /// A class to hold the Type information and Serialized payload for data stored in the cache.
    /// </summary>
    public class ADTOSharpCacheData
    {
        public ADTOSharpCacheData(
            string type, string payload)
        {
            Type = type;
            Payload = payload;
        }

        public string Payload { get; set; }

        public string Type { get; set; }

        public static ADTOSharpCacheData Deserialize(string serializedCacheData) => serializedCacheData.FromJsonString<ADTOSharpCacheData>();

        public static ADTOSharpCacheData Serialize(object obj, bool withAssemblyName = true)
        {
            return new ADTOSharpCacheData(
                TypeHelper.SerializeType(obj.GetType(), withAssemblyName).ToString(),
                obj.ToJsonString());
        }
        
    }
}
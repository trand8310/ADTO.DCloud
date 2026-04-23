using ADTOSharp.Dependency;
using ADTOSharp.Json;
using ADTOSharp.Json.SystemTextJson;
using StackExchange.Redis;
using System;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ADTOSharp.Runtime.Caching.Redis
{
    /// <summary>
    ///     Default implementation uses JSON as the underlying persistence mechanism.
    /// </summary>
    public class DefaultRedisCacheSerializer : IRedisCacheSerializer, ITransientDependency
    {
        /// <summary>
        ///     Creates an instance of the object from its serialized string representation.
        /// </summary>
        /// <param name="objbyte">String representation of the object from the Redis server.</param>
        /// <returns>Returns a newly constructed object.</returns>
        /// <seealso cref="IRedisCacheSerializer{TSource, TDestination}.Serialize" />
        public virtual object Deserialize(RedisValue objbyte)
        {
            var serializerSettings = new JsonSerializerOptions();
            serializerSettings.Converters.Insert(0, new ADTOSharp.Json.SystemTextJson.ADTOSharpDateTimeConverter());
            serializerSettings.Converters.Add(new ADTOSharpJsonConverterForType());

            var cacheData = ADTOSharpCacheData.Deserialize(objbyte);

            return cacheData.Payload.FromJsonString(
                Type.GetType(cacheData.Type, true, true),
                serializerSettings);
        }

        /// <summary>
        ///     Produce a string representation of the supplied object.
        /// </summary>
        /// <param name="value">Instance to serialize.</param>
        /// <param name="type">Type of the object.</param>
        /// <returns>Returns a string representing the object instance that can be placed into the Redis cache.</returns>
        /// <seealso cref="IRedisCacheSerializer{TSource, TDestination}.Deserialize" />
        public virtual RedisValue Serialize(object value, Type type)
        {
            var json = ADTOSharpCacheData.Serialize(value);
            return JsonSerializer.Serialize(json, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }
    }
}
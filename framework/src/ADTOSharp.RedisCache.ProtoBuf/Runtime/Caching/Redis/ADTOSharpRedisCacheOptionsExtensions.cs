using ADTOSharp.Configuration.Startup;
using ADTOSharp.Dependency;

namespace ADTOSharp.Runtime.Caching.Redis
{
    public static class ADTOSharpRedisCacheOptionsExtensions
    {
        public static void UseProtoBuf(this ADTOSharpRedisCacheOptions options)
        {
            options.ADTOSharpStartupConfiguration
                .ReplaceService<IRedisCacheSerializer, ProtoBufRedisCacheSerializer>(DependencyLifeStyle.Transient);

        }
    }
}

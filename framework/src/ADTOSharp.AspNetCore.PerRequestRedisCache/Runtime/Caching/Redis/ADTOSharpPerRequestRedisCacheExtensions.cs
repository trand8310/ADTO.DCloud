using System;
using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Caching.Configuration;

namespace ADTOSharp.Runtime.Caching.Redis;

public static class ADTOSharpPerRequestRedisCacheExtensions
{
    /// <summary>
    /// Configures caching to use Redis as cache server.
    /// </summary>
    /// <param name="cachingConfiguration">The caching configuration.</param>
    /// <param name="usePerRequestRedisCache">Replaces ICacheManager with <see cref="ADTOSharp.Runtime.Caching.Redis.ADTOSharpPerRequestRedisCacheManager"/></param>
    public static void UseRedis(this ICachingConfiguration cachingConfiguration, bool usePerRequestRedisCache)
    {
        cachingConfiguration.UseRedis(options => { }, usePerRequestRedisCache);
    }

    /// <summary>
    /// Configures caching to use Redis as cache server.
    /// </summary>
    /// <param name="cachingConfiguration">The caching configuration.</param>
    /// <param name="optionsAction">Action to get/set options</param>
    /// <param name="usePerRequestRedisCache">Replaces ICacheManager with <see cref="ADTOSharp.Runtime.Caching.Redis.ADTOSharpPerRequestRedisCacheManager"/></param>
    public static void UseRedis(this ICachingConfiguration cachingConfiguration, Action<ADTOSharpRedisCacheOptions> optionsAction, bool usePerRequestRedisCache)
    {
        if (!usePerRequestRedisCache)
        {
            cachingConfiguration.UseRedis(optionsAction);
            return;
        }

        var iocManager = cachingConfiguration.ADTOSharpConfiguration.IocManager;
        iocManager.RegisterIfNot<ICacheManager, ADTOSharpPerRequestRedisCacheManagerForReplacement>();

        optionsAction(iocManager.Resolve<ADTOSharpRedisCacheOptions>());
    }
}
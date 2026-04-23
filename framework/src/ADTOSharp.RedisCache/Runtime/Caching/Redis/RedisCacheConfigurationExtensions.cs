using System;
using ADTOSharp.Dependency;
using ADTOSharp.RealTime;
using ADTOSharp.Runtime.Caching.Configuration;
using ADTOSharp.Runtime.Caching.Redis.RealTime;

namespace ADTOSharp.Runtime.Caching.Redis
{
    /// <summary>
    /// Extension methods for <see cref="ICachingConfiguration"/>.
    /// </summary>
    public static class RedisCacheConfigurationExtensions
    {
        /// <summary>
        /// Configures caching to use Redis as cache server.
        /// </summary>
        /// <param name="cachingConfiguration">The caching configuration.</param>
        public static void UseRedis(this ICachingConfiguration cachingConfiguration)
        {
            cachingConfiguration.UseRedis(options => { });
        }

        /// <summary>
        /// Configures caching to use Redis as cache server.
        /// </summary>
        /// <param name="cachingConfiguration">The caching configuration.</param>
        /// <param name="optionsAction">Ac action to get/set options</param>
        public static void UseRedis(this ICachingConfiguration cachingConfiguration, Action<ADTOSharpRedisCacheOptions> optionsAction)
        {
            var iocManager = cachingConfiguration.ADTOSharpConfiguration.IocManager;

            iocManager.RegisterIfNot<ICacheManager, ADTOSharpRedisCacheManager>();
            iocManager.RegisterIfNot<IOnlineClientStore, RedisOnlineClientStore>();
            iocManager.RegisterIfNot<RedisOnlineClientStore, RedisOnlineClientStore>();
            
            optionsAction(iocManager.Resolve<ADTOSharpRedisCacheOptions>());
        }
    }
}

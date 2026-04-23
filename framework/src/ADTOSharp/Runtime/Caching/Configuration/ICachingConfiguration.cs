using System;
using System.Collections.Generic;
using ADTOSharp.Configuration.Startup;
using Microsoft.Extensions.Caching.Memory;

namespace ADTOSharp.Runtime.Caching.Configuration
{
    /// <summary>
    /// Used to configure caching system.
    /// </summary>
    public interface ICachingConfiguration
    {
        /// <summary>
        /// Gets the ADTO configuration object.
        /// </summary>
        IADTOSharpStartupConfiguration ADTOSharpConfiguration { get; }

        /// <summary>
        /// List of all registered configurators.
        /// </summary>
        IReadOnlyList<ICacheConfigurator> Configurators { get; }

        /// <summary>
        /// Options for memory cache
        /// </summary>
        MemoryCacheOptions MemoryCacheOptions { get; set; }

        /// <summary>
        /// Used to configure all caches.
        /// </summary>
        /// <param name="initAction">
        /// An action to configure caches
        /// This action is called for each cache just after created.
        /// </param>
        void ConfigureAll(Action<ICacheOptions> initAction);

        /// <summary>
        /// Used to configure a specific cache. 
        /// </summary>
        /// <param name="cacheName">Cache name</param>
        /// <param name="initAction">
        /// An action to configure the cache.
        /// This action is called just after the cache is created.
        /// </param>
        void Configure(string cacheName, Action<ICacheOptions> initAction);
    }
}

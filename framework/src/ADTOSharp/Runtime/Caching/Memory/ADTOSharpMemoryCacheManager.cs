using System.Linq;
using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Caching.Configuration;
using Castle.Core.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ADTOSharp.Runtime.Caching.Memory
{
    /// <summary>
    /// Implements <see cref="ICacheManager"/> to work with MemoryCache.
    /// </summary>
    public class ADTOSharpMemoryCacheManager : CacheManagerBase<ICache>, ICacheManager
    {
        public ILogger Logger { get; set; }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public ADTOSharpMemoryCacheManager(ICachingConfiguration configuration)
            : base(configuration)
        {
            Logger = NullLogger.Instance;
        }

        protected override ICache CreateCacheImplementation(string name)
        {
            return new ADTOSharpMemoryCache(name, Configuration?.ADTOSharpConfiguration?.Caching?.MemoryCacheOptions)
            {
                Logger = Logger
            };
        }

        protected override void DisposeCaches()
        {
            foreach (var cache in Caches.Values)
            {
                cache.Dispose();
            }
        }
    }
}

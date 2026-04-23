using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Caching.Configuration;

namespace ADTOSharp.Runtime.Caching.Redis
{
    /// <summary>
    /// Used to create <see cref="ADTOSharpRedisCache"/> instances.
    /// </summary>
    public class ADTOSharpRedisCacheManager : CacheManagerBase<ICache>, ICacheManager
    {
        private readonly IIocManager _iocManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ADTOSharpRedisCacheManager"/> class.
        /// </summary>
        public ADTOSharpRedisCacheManager(IIocManager iocManager, ICachingConfiguration configuration)
            : base(configuration)
        {
            _iocManager = iocManager;
            _iocManager.RegisterIfNot<ADTOSharpRedisCache>(DependencyLifeStyle.Transient);
        }

        protected override ICache CreateCacheImplementation(string name)
        {
            return _iocManager.Resolve<ADTOSharpRedisCache>(new { name });
        }
        protected override void DisposeCaches()
        {
            foreach (var cache in Caches)
            {
                _iocManager.Release(cache.Value);
            }
        }
    }
}

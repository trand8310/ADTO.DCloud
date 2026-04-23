using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Caching.Configuration;

namespace ADTOSharp.Runtime.Caching.Redis;

/// <summary>
/// Used to create <see cref="ADTOSharpPerRequestRedisCache"/> instances.
/// </summary>
public class ADTOSharpPerRequestRedisCacheManager : CacheManagerBase<ICache>, IADTOSharpPerRequestRedisCacheManager
{
    private readonly IIocManager _iocManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="ADTOSharpPerRequestRedisCacheManager"/> class.
    /// </summary>
    public ADTOSharpPerRequestRedisCacheManager(IIocManager iocManager, ICachingConfiguration configuration)
        : base(configuration)
    {
        _iocManager = iocManager;
        _iocManager.RegisterIfNot<ADTOSharpPerRequestRedisCache>(DependencyLifeStyle.Transient);
    }

    protected override ICache CreateCacheImplementation(string name)
    {
        return _iocManager.Resolve<ADTOSharpPerRequestRedisCache>(new { name });
    }

    protected override void DisposeCaches()
    {
        foreach (var cache in Caches)
        {
            _iocManager.Release(cache.Value);
        }
    }
}

internal class ADTOSharpPerRequestRedisCacheManagerForReplacement : ADTOSharpPerRequestRedisCacheManager
{
    public ADTOSharpPerRequestRedisCacheManagerForReplacement(IIocManager iocManager, ICachingConfiguration configuration) : base(iocManager, configuration)
    {
    }
}
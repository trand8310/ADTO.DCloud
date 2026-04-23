using ADTOSharp.Runtime.Caching;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ADTO.OpenIddict;

public class ADTOOpenIddictCacheBase<TEntity, TModel, TStore>
    where TModel : class
    where TEntity : class
{
    public ILogger<ADTOOpenIddictCacheBase<TEntity, TModel, TStore>> Logger { get; set; }
    protected ITypedCache<string, TModel> Cache { get; }
    protected ITypedCache<string, TModel[]> ArrayCache { get; }

    protected TStore Store { get; }
    protected ADTOOpenIddictCacheBase(ITypedCache<string, TModel> cache, ITypedCache<string, TModel[]> arrayCache, TStore store)
    {
        Cache = cache;
        ArrayCache = arrayCache;
        Store = store;
        Logger = NullLogger<ADTOOpenIddictCacheBase<TEntity, TModel, TStore>>.Instance;
    }
}

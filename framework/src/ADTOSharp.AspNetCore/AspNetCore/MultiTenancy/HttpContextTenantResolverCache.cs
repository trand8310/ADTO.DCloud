using ADTOSharp.Dependency;
using ADTOSharp.MultiTenancy;
using Microsoft.AspNetCore.Http;

namespace ADTOSharp.AspNetCore.MultiTenancy;

public class HttpContextTenantResolverCache : ITenantResolverCache, ITransientDependency
{
    private const string CacheItemKey = "ADTOSharp.MultiTenancy.TenantResolverCacheItem";

    public TenantResolverCacheItem Value
    {
        get
        {
            return _httpContextAccessor.HttpContext?.Items[CacheItemKey] as TenantResolverCacheItem;
        }

        set
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return;
            }

            httpContext.Items[CacheItemKey] = value;
        }
    }

    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextTenantResolverCache(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
}
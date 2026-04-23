using System;
using System.Threading.Tasks;

namespace ADTOSharp.MultiTenancy
{
    public interface ITenantCache
    {
        TenantCacheItem Get(Guid tenantId);

        TenantCacheItem Get(string tenancyName);

        TenantCacheItem GetOrNull(string tenancyName);

        TenantCacheItem GetOrNull(Guid tenantId);

        Task<TenantCacheItem> GetAsync(Guid tenantId);

        Task<TenantCacheItem> GetAsync(string tenancyName);

        Task<TenantCacheItem> GetOrNullAsync(string tenancyName);

        Task<TenantCacheItem> GetOrNullAsync(Guid tenantId);
    }
}

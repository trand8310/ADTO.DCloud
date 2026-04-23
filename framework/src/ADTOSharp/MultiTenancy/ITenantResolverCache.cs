using JetBrains.Annotations;

namespace ADTOSharp.MultiTenancy
{
    public interface ITenantResolverCache
    {
        [CanBeNull]
        TenantResolverCacheItem Value { get; set; }
    }
}
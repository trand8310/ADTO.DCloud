using ADTOSharp.Application.Editions;
using ADTOSharp.Authorization.Roles;
using ADTOSharp.Authorization.Users;
using ADTOSharp.MultiTenancy;
using System;

namespace ADTOSharp.Runtime.Caching
{
    public static class ADTOSharpZeroCacheManagerExtensions
    {
        public static ITypedCache<string, UserPermissionCacheItem> GetUserPermissionCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, UserPermissionCacheItem>(UserPermissionCacheItem.CacheStoreName);
        }

        public static ITypedCache<string, RolePermissionCacheItem> GetRolePermissionCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<string, RolePermissionCacheItem>(RolePermissionCacheItem.CacheStoreName);
        }

        public static ITypedCache<Guid, TenantFeatureCacheItem> GetTenantFeatureCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<Guid, TenantFeatureCacheItem>(TenantFeatureCacheItem.CacheStoreName);
        }

        public static ITypedCache<Guid, EditionfeatureCacheItem> GetEditionFeatureCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache<Guid, EditionfeatureCacheItem>(EditionfeatureCacheItem.CacheStoreName);
        }
    }
}

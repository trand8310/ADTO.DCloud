using System;
using System.Threading.Tasks;
using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.Runtime.Session;

namespace ADTOSharp.CachedUniqueKeys
{
    public class CachedUniqueKeyPerUser : ICachedUniqueKeyPerUser, ITransientDependency
    {
        public IADTOSharpSession ADTOSharpSession { get; set; }

        private readonly ICacheManager _cacheManager;

        public CachedUniqueKeyPerUser(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            ADTOSharpSession = NullADTOSharpSession.Instance;
        }

        public virtual Task<string> GetKeyAsync(string cacheName)
        {
            return GetKeyAsync(cacheName, ADTOSharpSession.TenantId, ADTOSharpSession.UserId);
        }

        public virtual Task RemoveKeyAsync(string cacheName)
        {
            return RemoveKeyAsync(cacheName, ADTOSharpSession.TenantId, ADTOSharpSession.UserId);
        }

        public virtual Task<string> GetKeyAsync(string cacheName, UserIdentifier user)
        {
            return GetKeyAsync(cacheName, user.TenantId, user.UserId);
        }

        public virtual Task RemoveKeyAsync(string cacheName, UserIdentifier user)
        {
            return RemoveKeyAsync(cacheName, user.TenantId, user.UserId);
        }

        public virtual async Task<string> GetKeyAsync(string cacheName, Guid? tenantId, Guid? userId)
        {
            if (!ADTOSharpSession.UserId.HasValue)
            {
                return Guid.NewGuid().ToString("N");
            }

            var cache = GetCache(cacheName);
            return await cache.GetAsync(GetCacheKeyForUser(tenantId, userId),
                () => Task.FromResult(Guid.NewGuid().ToString("N")));
        }

        public virtual async Task RemoveKeyAsync(string cacheName, Guid? tenantId, Guid? userId)
        {
            if (!ADTOSharpSession.UserId.HasValue)
            {
                return;
            }

            var cache = GetCache(cacheName);
            await cache.RemoveAsync(GetCacheKeyForUser(tenantId, userId));
        }

        public virtual async Task ClearCacheAsync(string cacheName)
        {
            var cache = GetCache(cacheName);
            await cache.ClearAsync();
        }

        public virtual string GetKey(string cacheName)
        {
            return GetKey(cacheName, ADTOSharpSession.TenantId, ADTOSharpSession.UserId);
        }

        public virtual void RemoveKey(string cacheName)
        {
            RemoveKey(cacheName, ADTOSharpSession.TenantId, ADTOSharpSession.UserId);
        }

        public virtual string GetKey(string cacheName, UserIdentifier user)
        {
            return GetKey(cacheName, user.TenantId, user.UserId);
        }

        public virtual void RemoveKey(string cacheName, UserIdentifier user)
        {
            RemoveKey(cacheName, user.TenantId, user.UserId);
        }

        public virtual string GetKey(string cacheName, Guid? tenantId, Guid? userId)
        {
            if (!ADTOSharpSession.UserId.HasValue)
            {
                return Guid.NewGuid().ToString("N");
            }

            var cache = GetCache(cacheName);
            return cache.Get(GetCacheKeyForUser(tenantId, userId),
                () => Guid.NewGuid().ToString("N"));
        }

        public virtual void RemoveKey(string cacheName, Guid? tenantId, Guid? userId)
        {
            if (!ADTOSharpSession.UserId.HasValue)
            {
                return;
            }

            var cache = GetCache(cacheName);
            cache.Remove(GetCacheKeyForUser(tenantId, userId));
        }

        public virtual void ClearCache(string cacheName)
        {
            var cache = GetCache(cacheName);
            cache.Clear();
        }

        protected virtual ITypedCache<string, string> GetCache(string cacheName)
        {
            return _cacheManager.GetCache<string, string>(cacheName);
        }

        protected virtual string GetCacheKeyForUser(Guid? tenantId, Guid? userId)
        {
            if (tenantId == null)
            {
                return userId.ToString();
            }

            return userId + "@" + tenantId;
        }
    }
}
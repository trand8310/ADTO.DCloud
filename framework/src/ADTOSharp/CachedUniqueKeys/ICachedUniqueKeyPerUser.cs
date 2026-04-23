using System;
using System.Threading.Tasks;

namespace ADTOSharp.CachedUniqueKeys
{
    public interface ICachedUniqueKeyPerUser
    {
        Task<string> GetKeyAsync(string cacheName);

        Task RemoveKeyAsync(string cacheName);
        
        Task<string> GetKeyAsync(string cacheName, UserIdentifier user);

        Task RemoveKeyAsync(string cacheName, UserIdentifier user);

        Task<string> GetKeyAsync(string cacheName, Guid? tenantId, Guid? userId);

        Task RemoveKeyAsync(string cacheName, Guid? tenantId, Guid? userId);

        Task ClearCacheAsync(string cacheName);

        string GetKey(string cacheName);

        void RemoveKey(string cacheName);
        
        string GetKey(string cacheName, UserIdentifier user);

        void RemoveKey(string cacheName, UserIdentifier user);

        string GetKey(string cacheName, Guid? tenantId, Guid? userId);

        void RemoveKey(string cacheName, Guid? tenantId, Guid? userId);

        void ClearCache(string cacheName);
    }
}
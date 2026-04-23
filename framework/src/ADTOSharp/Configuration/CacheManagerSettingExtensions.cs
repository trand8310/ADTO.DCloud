using System;
using System.Collections.Generic;
using ADTOSharp.Runtime.Caching;

namespace ADTOSharp.Configuration
{
    /// <summary>
    /// Extension methods for <see cref="ICacheManager"/> to get setting caches.
    /// </summary>
    public static class CacheManagerSettingExtensions
    {
        /// <summary>
        /// Gets application settings cache.
        /// </summary>
        public static ITypedCache<string, Dictionary<string, SettingInfo>> GetApplicationSettingsCache(this ICacheManager cacheManager)
        {
            return cacheManager
                .GetCache<string, Dictionary<string, SettingInfo>>(ADTOSharpCacheNames.ApplicationSettings);
        }

        /// <summary>
        /// Gets tenant settings cache.
        /// </summary>
        public static ITypedCache<Guid, Dictionary<string, SettingInfo>> GetTenantSettingsCache(this ICacheManager cacheManager)
        {
            return cacheManager
                .GetCache<Guid, Dictionary<string, SettingInfo>>(ADTOSharpCacheNames.TenantSettings);
        }

        /// <summary>
        /// Gets user settings cache.
        /// </summary>
        public static ITypedCache<string, Dictionary<string, SettingInfo>> GetUserSettingsCache(this ICacheManager cacheManager)
        {
            return cacheManager
                .GetCache<string, Dictionary<string, SettingInfo>>(ADTOSharpCacheNames.UserSettings);
        }
    }
}

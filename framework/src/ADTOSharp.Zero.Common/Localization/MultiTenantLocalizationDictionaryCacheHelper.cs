using ADTOSharp.Runtime.Caching;
using System;
using System.Collections.Generic;

namespace ADTOSharp.Localization
{
    /// <summary>
    /// A helper to implement localization cache.
    /// </summary>
    public static class MultiTenantLocalizationDictionaryCacheHelper
    {
        /// <summary>
        /// The cache name.
        /// </summary>
        public const string CacheName = "ADTOSharpZeroMultiTenantLocalizationDictionaryCache";

        public static ITypedCache<string, OrderedDictionary<string, string>> GetMultiTenantLocalizationDictionaryCache(this ICacheManager cacheManager)
        {
            return cacheManager.GetCache(CacheName).AsTyped<string, OrderedDictionary<string, string>>();
        }
 

        public static string CalculateCacheKey(Guid? tenantId, string sourceName, string languageName)
        {
            return sourceName + "#" + languageName + "#" + (tenantId ?? Guid.Empty);
        }
    }
}
using System;
using System.Collections.Generic;

namespace ADTOSharp.MultiTenancy
{
    /// <summary>
    /// Used to store features of a Tenant in the cache.
    /// </summary>
    [Serializable]
    public class TenantFeatureCacheItem
    {
        /// <summary>
        /// The cache store name.
        /// </summary>
        public const string CacheStoreName = "ADTOSharpZeroTenantFeatures";

        /// <summary>
        /// Edition of the tenant.
        /// </summary>
        public Guid? EditionId { get; set; }

        /// <summary>
        /// Feature values.
        /// </summary>
        public IDictionary<string, string> FeatureValues { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantFeatureCacheItem"/> class.
        /// </summary>
        public TenantFeatureCacheItem()
        {
            FeatureValues = new Dictionary<string, string>();
        }
    }
}
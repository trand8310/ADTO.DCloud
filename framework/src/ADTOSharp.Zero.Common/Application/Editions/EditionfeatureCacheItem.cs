using System;
using System.Collections.Generic;

namespace ADTOSharp.Application.Editions
{
    [Serializable]
    public class EditionfeatureCacheItem
    {
        public const string CacheStoreName = "ADTOSharpZeroEditionFeatures";

        public IDictionary<string, string> FeatureValues { get; set; }

        public EditionfeatureCacheItem()
        {
            FeatureValues = new Dictionary<string, string>();
        }
    }
}
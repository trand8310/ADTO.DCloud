namespace ADTOSharp.Runtime.Caching.Redis
{
    public class ADTOSharpRedisCacheKeyNormalizeArgs
    {
        public string Key { get; }

        public string CacheName { get; }

        public bool MultiTenancyEnabled { get; }

        public ADTOSharpRedisCacheKeyNormalizeArgs(
            string key,
            string cacheName,
            bool multiTenancyEnabled)
        {
            Key = key;
            CacheName = cacheName;
            MultiTenancyEnabled = multiTenancyEnabled;
        }
    }
}

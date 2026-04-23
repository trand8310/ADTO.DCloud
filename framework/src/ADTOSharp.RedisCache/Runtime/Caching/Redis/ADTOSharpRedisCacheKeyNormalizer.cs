using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Session;
using Microsoft.Extensions.Options;

namespace ADTOSharp.Runtime.Caching.Redis
{
    public class ADTOSharpRedisCacheKeyNormalizer : IADTOSharpRedisCacheKeyNormalizer, ITransientDependency
    {
        public IADTOSharpSession ADTOSharpSession { get; set; }
        protected ADTOSharpRedisCacheOptions RedisCacheOptions { get; }

        public ADTOSharpRedisCacheKeyNormalizer(
        IOptions<ADTOSharpRedisCacheOptions> redisCacheOptions)
        {
            ADTOSharpSession = NullADTOSharpSession.Instance;
            RedisCacheOptions = redisCacheOptions.Value;
        }

        public string NormalizeKey(ADTOSharpRedisCacheKeyNormalizeArgs args)
        {
            var normalizedKey = $"n:{args.CacheName},c:{RedisCacheOptions.KeyPrefix}{args.Key}";

            if (args.MultiTenancyEnabled && ADTOSharpSession.TenantId != null && RedisCacheOptions.TenantKeyEnabled)
            {
                normalizedKey = $"t:{ADTOSharpSession.TenantId},{normalizedKey}";
            }

            return normalizedKey;
        }
    }
}

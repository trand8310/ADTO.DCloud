

using ADTO.DCloud.Authentication.External;
using ADTOSharp.Runtime.Caching;

namespace ADTO.DCloud.Web.Host.Startup.ExternalLoginInfoProviders;

public static class ExternalLoginInfoProvidersCacheManagerExtensions
{
    private const string CacheName = "AppExternalLoginInfoProvidersCache";
    public static ITypedCache<string, ExternalLoginProviderInfo> GetExternalLoginInfoProviderCache(this ICacheManager cacheManager)
    {
        return cacheManager.GetCache<string, ExternalLoginProviderInfo>(CacheName);
    }
}
using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Caching;
using ADTO.DCloud.Configuration;
using ADTOSharp.Runtime.Session;
using ADTO.DCloud.Authentication.External.Weixin;


namespace ADTO.DCloud.Web.Host.Startup.ExternalLoginInfoProviders;

public class ExternalLoginOptionsCacheManager : IExternalLoginOptionsCacheManager, ITransientDependency
{
    private readonly ICacheManager _cacheManager;
    private readonly IADTOSharpSession _session;

    public ExternalLoginOptionsCacheManager(ICacheManager cacheManager, IADTOSharpSession session)
    {
        _cacheManager = cacheManager;
        _session = session;
    }

    public void ClearCache()
    {
        _cacheManager.GetExternalLoginInfoProviderCache().Remove(GetCacheKey(WeixinMiniProgramAuthProviderApi.Name));
    }

    private string GetCacheKey(string name)
    {
        if (_session.TenantId.HasValue)
        {
            return $"{name}-{_session.TenantId.Value}";
        }

        return $"{name}";
    }
}


using ADTO.DCloud.Authentication.External;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.Runtime.Session;

namespace ADTO.DCloud.Web.Host.Startup.ExternalLoginInfoProviders;

public abstract class TenantBasedExternalLoginInfoProviderBase : IExternalLoginInfoProvider
{
    private readonly IADTOSharpSession _session;
    private readonly ICacheManager _cacheManager;
    public abstract string Name { get; }

    protected TenantBasedExternalLoginInfoProviderBase(
        IADTOSharpSession session,
        ICacheManager cacheManager)
    {
        _session = session;
        _cacheManager = cacheManager;
    }

    protected abstract bool TenantHasSettings();

    protected abstract ExternalLoginProviderInfo GetTenantInformation();

    protected abstract ExternalLoginProviderInfo GetHostInformation();

    public virtual ExternalLoginProviderInfo GetExternalLoginInfo()
    {
        if (_session.TenantId.HasValue && TenantHasSettings())
        {
            return _cacheManager.GetExternalLoginInfoProviderCache()
                .Get(GetCacheKey(), GetTenantInformation);
        }

        return _cacheManager.GetExternalLoginInfoProviderCache()
                .Get(GetCacheKey(), GetHostInformation);
    }

    private string GetCacheKey()
    {
        if (_session.TenantId.HasValue)
        {
            return $"{Name}-{_session.TenantId.Value}";
        }

        return $"{Name}";
    }
}

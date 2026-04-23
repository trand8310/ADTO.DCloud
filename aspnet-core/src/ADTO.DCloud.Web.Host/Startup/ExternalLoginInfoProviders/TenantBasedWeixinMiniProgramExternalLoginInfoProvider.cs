using ADTO.DCloud.Authentication;
using ADTO.DCloud.Authentication.External;
using ADTO.DCloud.Authentication.External.Weixin;
using ADTO.DCloud.Configuration;
using ADTOSharp.Configuration;
using ADTOSharp.Dependency;
using ADTOSharp.Extensions;
using ADTOSharp.Json;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.Runtime.Session;

namespace ADTO.DCloud.Web.Host.Startup.ExternalLoginInfoProviders;



public class TenantBasedWeixinMiniProgramExternalLoginInfoProvider : TenantBasedExternalLoginInfoProviderBase, ISingletonDependency
{
    private readonly ISettingManager _settingManager;
    private readonly IADTOSharpSession _session;
    public override string Name { get; } = WeixinMiniProgramAuthProviderApi.Name;

    public TenantBasedWeixinMiniProgramExternalLoginInfoProvider(
        ISettingManager settingManager,
        IADTOSharpSession session,
        ICacheManager cacheManager) : base(session, cacheManager)
    {
        _settingManager = settingManager;
        _session = session;
    }

    private ExternalLoginProviderInfo CreateExternalLoginInfo(WeixinMiniProgramExternalLoginProviderSettings settings)
    {
        return new ExternalLoginProviderInfo(Name, settings.AppId, settings.AppSecret, typeof(WeixinMiniProgramAuthProviderApi));
    }

    protected override bool TenantHasSettings()
    {
        var settingValue = _settingManager.GetSettingValueForTenant(AppSettings.ExternalLoginProvider.Tenant.WeixinMiniProgram, _session.TenantId.Value);
        return !settingValue.IsNullOrWhiteSpace();
    }

    protected override ExternalLoginProviderInfo GetTenantInformation()
    {
        string settingValue = _settingManager.GetSettingValueForTenant(AppSettings.ExternalLoginProvider.Tenant.WeixinMiniProgram, _session.TenantId.Value);
        var settings = settingValue.FromJsonString<WeixinMiniProgramExternalLoginProviderSettings>();
        return CreateExternalLoginInfo(settings);
    }

    protected override ExternalLoginProviderInfo GetHostInformation()
    {
        string settingValue = _settingManager.GetSettingValueForApplication(AppSettings.ExternalLoginProvider.Host.WeixinMiniProgram);
        var settings = settingValue.FromJsonString<WeixinMiniProgramExternalLoginProviderSettings>();
        return CreateExternalLoginInfo(settings);
    }
}

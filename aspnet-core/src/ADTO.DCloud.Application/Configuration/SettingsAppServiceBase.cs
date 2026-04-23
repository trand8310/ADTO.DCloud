using System;
using System.Threading.Tasks;
using ADTOSharp.Net.Mail;
using ADTOSharp.UI;
using Microsoft.Extensions.Configuration;
using ADTO.DCloud.Configuration.Dto;
using ADTO.DCloud.Configuration.Host.Dto;


namespace ADTO.DCloud.Configuration;

/// <summary>
/// 设置服务抽像基类
/// </summary>
public abstract class SettingsAppServiceBase : DCloudAppServiceBase
{
    private readonly IEmailSender _emailSender;
    private readonly IAppConfigurationAccessor _configurationAccessor;

    protected SettingsAppServiceBase(
        IEmailSender emailSender,
        IAppConfigurationAccessor configurationAccessor)
    {
        _emailSender = emailSender;
        _configurationAccessor = configurationAccessor;
    }

    #region Send Test Email

    public async Task SendTestEmail(SendTestEmailInput input)
    {
        try
        {
            await _emailSender.SendAsync(
                input.EmailAddress,
                L("TestEmail_Subject"),
                L("TestEmail_Body")
            );
        }
        catch (Exception e)
        {
            throw new UserFriendlyException("An error was encountered while sending an email. " + e.Message, e);
        }
    }
    /// <summary>
    /// 获取设置登录设置
    /// </summary>
    /// <returns></returns>
    public ExternalLoginSettingsDto GetEnabledSocialLoginSettings()
    {
        var dto = new ExternalLoginSettingsDto();
        if (!bool.Parse(_configurationAccessor.Configuration["Authentication:AllowSocialLoginSettingsPerTenant"]))
        {
            return dto;
        }

        if (IsSocialLoginEnabled("Facebook"))
        {
            dto.EnabledSocialLoginSettings.Add("Facebook");
        }

        if (IsSocialLoginEnabled("Google"))
        {
            dto.EnabledSocialLoginSettings.Add("Google");
        }

        if (IsSocialLoginEnabled("Twitter"))
        {
            dto.EnabledSocialLoginSettings.Add("Twitter");
        }

        if (IsSocialLoginEnabled("Microsoft"))
        {
            dto.EnabledSocialLoginSettings.Add("Microsoft");
        }

        if (IsSocialLoginEnabled("WsFederation"))
        {
            dto.EnabledSocialLoginSettings.Add("WsFederation");
        }

        if (IsSocialLoginEnabled("OpenId"))
        {
            dto.EnabledSocialLoginSettings.Add("OpenId");
        }

        return dto;
    }

    private bool IsSocialLoginEnabled(string name)
    {
        return _configurationAccessor.Configuration.GetSection("Authentication:" + name).Exists() &&
               bool.Parse(_configurationAccessor.Configuration["Authentication:" + name + ":IsEnabled"]);
    }

    #endregion
}

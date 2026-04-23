using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using ADTOSharp.Configuration;
using ADTOSharp.Extensions;
using ADTOSharp.Localization;
using ADTOSharp.Runtime.Session;
using Castle.Core.Logging;
using JetBrains.Annotations;

namespace ADTOSharp.AspNetCore.Localization;

public class ADTOSharpUserRequestCultureProvider : RequestCultureProvider
{
    public CookieRequestCultureProvider CookieProvider { get; set; }
    public ADTOSharpLocalizationHeaderRequestCultureProvider HeaderProvider { get; set; }
    public ILogger Logger { get; set; }

    public ADTOSharpUserRequestCultureProvider()
    {
        Logger = NullLogger.Instance;
    }

    public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var adtoSession = httpContext.RequestServices.GetRequiredService<IADTOSharpSession>();
        if (adtoSession.UserId == null)
        {
            return null;
        }

        var settingManager = httpContext.RequestServices.GetRequiredService<ISettingManager>();

        var userCulture = await settingManager.GetSettingValueForUserAsync(
            LocalizationSettingNames.DefaultLanguage,
            adtoSession.TenantId,
            adtoSession.UserId.Value,
            fallbackToDefault: false
        );

        if (!userCulture.IsNullOrEmpty())
        {
            Logger.DebugFormat("{0} - Read from user settings", nameof(ADTOSharpUserRequestCultureProvider));
            Logger.DebugFormat("Using Culture:{0} , UICulture:{1}", userCulture, userCulture);
            return new ProviderCultureResult(userCulture, userCulture);
        }

        ProviderCultureResult result = null;
        string cultureName = null;
        var cookieResult = await GetResultOrNull(httpContext, CookieProvider);
        if (cookieResult != null && cookieResult.Cultures.Any())
        {
            var cookieCulture = cookieResult.Cultures.First().Value;
            var cookieUICulture = cookieResult.UICultures.First().Value;

            Logger.DebugFormat("{0} - Read from cookie", nameof(ADTOSharpUserRequestCultureProvider));
            Logger.DebugFormat("Using Culture:{0} , UICulture:{1}", cookieCulture, cookieUICulture);

            result = cookieResult;
            cultureName = cookieCulture ?? cookieUICulture;
        }

        if (result == null || !result.Cultures.Any())
        {
            var headerResult = await GetResultOrNull(httpContext, HeaderProvider);
            if (headerResult != null && headerResult.Cultures.Any())
            {
                var headerCulture = headerResult.Cultures.First().Value;
                var headerUICulture = headerResult.UICultures.First().Value;

                Logger.DebugFormat("{0} - Read from header", nameof(ADTOSharpUserRequestCultureProvider));
                Logger.DebugFormat("Using Culture:{0} , UICulture:{1}", headerCulture, headerUICulture);

                result = headerResult;
                cultureName = headerCulture ?? headerUICulture;
            }
        }

        if (cultureName.IsNullOrEmpty() || cultureName == await GetDefaultCulture(adtoSession, settingManager))
        {
            return result;
        }

        if (GlobalizationHelper.IsValidCultureCode(cultureName))
        {
            // Try to set user's language setting from cookie/header if available and not default.
            await settingManager.ChangeSettingForUserAsync(
                adtoSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                cultureName
            );
        }

        return result;
    }

    protected virtual async Task<ProviderCultureResult> GetResultOrNull([NotNull] HttpContext httpContext, [CanBeNull] IRequestCultureProvider provider)
    {
        if (provider == null)
        {
            return null;
        }

        return await provider.DetermineProviderCultureResult(httpContext);
    }

    private Task<string> GetDefaultCulture(IADTOSharpSession adtoSession, ISettingManager settingManager)
    {
        return adtoSession.TenantId.HasValue
            ? settingManager.GetSettingValueForTenantAsync(LocalizationSettingNames.DefaultLanguage, adtoSession.TenantId.Value)
            : settingManager.GetSettingValueForApplicationAsync(LocalizationSettingNames.DefaultLanguage);
    }
}
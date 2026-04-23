using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using ADTOSharp.Configuration;
using ADTOSharp.Extensions;
using ADTOSharp.Localization;
using Castle.Core.Logging;

namespace ADTOSharp.AspNetCore.Localization;

public class ADTOSharpDefaultRequestCultureProvider : RequestCultureProvider
{
    public ILogger Logger { get; set; }

    public ADTOSharpDefaultRequestCultureProvider()
    {
        Logger = NullLogger.Instance;
    }

    public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var settingManager = httpContext.RequestServices.GetRequiredService<ISettingManager>();

        var culture = await settingManager.GetSettingValueAsync(LocalizationSettingNames.DefaultLanguage);

        if (culture.IsNullOrEmpty())
        {
            return null;
        }

        Logger.DebugFormat("{0} - Using Culture:{1} , UICulture:{2}", nameof(ADTOSharpDefaultRequestCultureProvider), culture, culture);
        return new ProviderCultureResult(culture, culture);
    }
}
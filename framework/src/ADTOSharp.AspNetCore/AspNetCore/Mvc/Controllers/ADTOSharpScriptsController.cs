using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ADTOSharp.Auditing;
using ADTOSharp.Extensions;
using ADTOSharp.Localization;
using ADTOSharp.Web.Authorization;
using ADTOSharp.Web.Configuration;
using ADTOSharp.Web.Features;
using ADTOSharp.Web.Localization;
using ADTOSharp.Web.Minifier;
using ADTOSharp.Web.MultiTenancy;
using ADTOSharp.Web.Navigation;
using ADTOSharp.Web.Sessions;
using ADTOSharp.Web.Settings;
using ADTOSharp.Web.Timing;
using Microsoft.AspNetCore.Mvc;

namespace ADTOSharp.AspNetCore.Mvc.Controllers;

/// <summary>
/// This controller is used to create client side scripts
/// to work with ADTO.
/// </summary>
public class ADTOSharpScriptsController : ADTOSharpController
{
    private readonly IMultiTenancyScriptManager _multiTenancyScriptManager;
    private readonly ISettingScriptManager _settingScriptManager;
    private readonly INavigationScriptManager _navigationScriptManager;
    private readonly ILocalizationScriptManager _localizationScriptManager;
    private readonly IAuthorizationScriptManager _authorizationScriptManager;
    private readonly IFeaturesScriptManager _featuresScriptManager;
    private readonly ISessionScriptManager _sessionScriptManager;
    private readonly ITimingScriptManager _timingScriptManager;
    private readonly ICustomConfigScriptManager _customConfigScriptManager;
    private readonly IJavaScriptMinifier _javaScriptMinifier;

    /// <summary>
    /// Constructor.
    /// </summary>
    public ADTOSharpScriptsController(
        IMultiTenancyScriptManager multiTenancyScriptManager,
        ISettingScriptManager settingScriptManager,
        INavigationScriptManager navigationScriptManager,
        ILocalizationScriptManager localizationScriptManager,
        IAuthorizationScriptManager authorizationScriptManager,
        IFeaturesScriptManager featuresScriptManager,
        ISessionScriptManager sessionScriptManager,
        ITimingScriptManager timingScriptManager,
        ICustomConfigScriptManager customConfigScriptManager,
        IJavaScriptMinifier javaScriptMinifier)
    {
        _multiTenancyScriptManager = multiTenancyScriptManager;
        _settingScriptManager = settingScriptManager;
        _navigationScriptManager = navigationScriptManager;
        _localizationScriptManager = localizationScriptManager;
        _authorizationScriptManager = authorizationScriptManager;
        _featuresScriptManager = featuresScriptManager;
        _sessionScriptManager = sessionScriptManager;
        _timingScriptManager = timingScriptManager;
        _customConfigScriptManager = customConfigScriptManager;
        _javaScriptMinifier = javaScriptMinifier;
    }

    /// <summary>
    /// Gets all needed scripts.
    /// </summary>
    [DisableAuditing]
    public async Task<ActionResult> GetScripts(string culture = "", bool minify = false)
    {
        if (!culture.IsNullOrEmpty())
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
        }

        var sb = new StringBuilder();

        sb.AppendLine(_multiTenancyScriptManager.GetScript());
        sb.AppendLine();

        sb.AppendLine(_sessionScriptManager.GetScript());
        sb.AppendLine();

        sb.AppendLine(_localizationScriptManager.GetScript());
        sb.AppendLine();

        sb.AppendLine(await _featuresScriptManager.GetScriptAsync());
        sb.AppendLine();

        sb.AppendLine(await _authorizationScriptManager.GetScriptAsync());
        sb.AppendLine();

        sb.AppendLine(await _navigationScriptManager.GetScriptAsync());
        sb.AppendLine();

        sb.AppendLine(await _settingScriptManager.GetScriptAsync());
        sb.AppendLine();

        sb.AppendLine(await _timingScriptManager.GetScriptAsync());
        sb.AppendLine();

        sb.AppendLine(_customConfigScriptManager.GetScript());
        sb.AppendLine();

        sb.AppendLine(GetTriggerScript());

        return Content(minify ? _javaScriptMinifier.Minify(sb.ToString()) : sb.ToString(),
            "application/x-javascript", Encoding.UTF8);
    }

    private static string GetTriggerScript()
    {
        var script = new StringBuilder();

        script.AppendLine("(function(){");
        script.AppendLine("    adto.event.trigger('adto.dynamicScriptsInitialized');");
        script.Append("})();");

        return script.ToString();
    }
}
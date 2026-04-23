using System;
using System.Collections.Generic;
using ADTOSharp.AspNetCore.Mvc.Extensions;
using ADTOSharp.Auditing;
using ADTOSharp.Configuration;
using ADTOSharp.Localization;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.Runtime.Session;
using ADTOSharp.Timing;
using ADTOSharp.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using IUrlHelper = ADTOSharp.Web.Http.IUrlHelper;

namespace ADTOSharp.AspNetCore.Mvc.Controllers;

public class ADTOSharpLocalizationController : ADTOSharpController
{
    protected IUrlHelper UrlHelper;
    private readonly ISettingStore _settingStore;

    private readonly ITypedCache<string, Dictionary<string, SettingInfo>> _userSettingCache;

    public ADTOSharpLocalizationController(
        IUrlHelper urlHelper,
        ISettingStore settingStore,
        ICacheManager cacheManager)
    {
        UrlHelper = urlHelper;
        _settingStore = settingStore;
        _userSettingCache = cacheManager.GetUserSettingsCache();
    }

    [DisableAuditing]
    public virtual ActionResult ChangeCulture(string cultureName, string returnUrl = "")
    {
        if (!GlobalizationHelper.IsValidCultureCode(cultureName))
        {
            throw new ADTOSharpException("Unknown language: " + cultureName + ". It must be a valid culture!");
        }

        var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cultureName, cultureName));

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            cookieValue,
            new CookieOptions
            {
                Expires = Clock.Now.AddYears(2),
                HttpOnly = true
            }
        );

        if (ADTOSharpSession.UserId.HasValue)
        {
            ChangeCultureForUser(cultureName);
        }

        if (Request.IsAjaxRequest())
        {
            return Json(new AjaxResponse());
        }

        if (!string.IsNullOrWhiteSpace(returnUrl))
        {
            var escapedReturnUrl = Uri.EscapeDataString(returnUrl);
            var localPath = UrlHelper.LocalPathAndQuery(escapedReturnUrl, Request.Host.Host, Request.Host.Port);
            if (!string.IsNullOrWhiteSpace(localPath))
            {
                var unescapedLocalPath = Uri.UnescapeDataString(localPath);
                if (Url.IsLocalUrl(unescapedLocalPath))
                {
                    return LocalRedirect(unescapedLocalPath);
                }
            }
        }

        return LocalRedirect("/");
    }

    protected virtual void ChangeCultureForUser(string cultureName)
    {
        var languageSetting = _settingStore.GetSettingOrNull(
            ADTOSharpSession.TenantId,
            ADTOSharpSession.GetUserId(),
            LocalizationSettingNames.DefaultLanguage
        );

        if (languageSetting == null)
        {
            _settingStore.Create(new SettingInfo(
                ADTOSharpSession.TenantId,
                ADTOSharpSession.UserId,
                LocalizationSettingNames.DefaultLanguage,
                cultureName
            ));
        }
        else
        {
            _settingStore.Update(new SettingInfo(
                ADTOSharpSession.TenantId,
                ADTOSharpSession.UserId,
                LocalizationSettingNames.DefaultLanguage,
                cultureName
            ));
        }

        _userSettingCache.Remove(ADTOSharpSession.ToUserIdentifier().ToString());
    }
}
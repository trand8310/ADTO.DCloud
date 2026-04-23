using ADTOSharp.AspNetCore.Mvc.Controllers;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.IdentityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ADTO.DCloud.Controllers;

/// <summary>
/// 控制器抽像基类,所有的控制器都要继承于这个类
/// </summary>
public abstract class DCloudControllerBase: ADTOSharpController
{
    protected DCloudControllerBase()
    {
        LocalizationSourceName = DCloudConsts.LocalizationSourceName;
    }

    protected void CheckErrors(IdentityResult identityResult)
    {
        identityResult.CheckErrors(LocalizationManager);
    }

    protected void SetTenantIdCookie(Guid? tenantId)
    {
        var multiTenancyConfig = HttpContext.RequestServices.GetRequiredService<IMultiTenancyConfig>();
        Response.Cookies.Append(
            multiTenancyConfig.TenantIdResolveKey,
            tenantId?.ToString() ?? string.Empty,
            new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddYears(5),
                Path = "/"
            }
        );
    }
}


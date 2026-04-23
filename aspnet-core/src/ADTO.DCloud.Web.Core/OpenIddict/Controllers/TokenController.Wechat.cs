using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.MultiTenancy;
using ADTO.DCloud.Web.Authentication.JwtBearer;
using ADTO.DCloud.Web.Models.TokenAuth;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Runtime.Security;
using ADTOSharp.Runtime.Session;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NUglify.Helpers;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ADTO.DCloud.Web.OpenIddict.Controllers;

/// <summary>
///  微信小程序登录实现
/// </summary>
public partial class TokenController
{

    protected virtual async Task<IActionResult> HandleWechatAsync(OpenIddictRequest request)
    {
        var session = IocManager.Instance.Resolve<IADTOSharpSession>();
        var uowManager = IocManager.Instance.Resolve<IUnitOfWorkManager>();
        var userManager = IocManager.Instance.Resolve<ADTOSharpUserManager<Role, User>>();
        var signInManager = IocManager.Instance.Resolve<ADTOSharpSignInManager<Tenant, Role, User>>();
        var logInManager = IocManager.Instance.Resolve<LogInManager>();
        var authProvider = request["authProvider"]?.ToString();
        var providerAccessCode = request["providerAccessCode"]?.ToString();

        return await uowManager.WithUnitOfWorkAsync(async () =>
        {
            var model = new Models.TokenAuth.ExternalAuthenticateModel() { AuthProvider = "WeixinMiniProgram", ProviderAccessCode = providerAccessCode };
            var externalUser = await GetExternalUserInfo(model);
            if (externalUser == null || externalUser.ProviderKey.IsNullOrWhiteSpace())
            {
                return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "无效的微信授权请求"
                }));
            }

            var loginResult = await logInManager.LoginAsync(
                new UserLoginInfo(model.AuthProvider, externalUser.ProviderKey, model.AuthProvider),
                GetTenancyNameOrNull()
            );

            switch (loginResult.Result)
            {
                case ADTOSharpLoginResultType.Success:
                    {
                        return await SetSuccessResultAsync(request, loginResult.User);
                    }
                case ADTOSharpLoginResultType.UnknownExternalLogin:
                    {
                        return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = "UnknownExternalLogin",
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "无效的外部用户"
                        }));
                    }
                default:
                    {

                        return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "微信小程序登录失败"
                        }));
                    }
            }
        });

















    }
}

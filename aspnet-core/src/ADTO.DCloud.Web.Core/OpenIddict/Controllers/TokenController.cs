using ADTO.AspNetCore.OpenIddict.Claims;
using ADTO.AspNetCore.OpenIddict.Controllers;
using ADTO.DCloud.Authentication.External;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.MultiTenancy;
using ADTO.DCloud.Web.Models.TokenAuth;
using ADTOSharp;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Extensions;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADTO.DCloud.Web.OpenIddict.Controllers
{
    public partial class TokenController : TokenController<Tenant, Role, User>
    {
        private readonly IExternalAuthManager _externalAuthManager;
        private readonly ITenantCache _tenantCache;
        public TokenController(ADTOSharpSignInManager<Tenant, Role, User> signInManager,
            ADTOSharpUserManager<Role, User> userManager, IOpenIddictApplicationManager applicationManager,
            IOpenIddictAuthorizationManager authorizationManager, IOpenIddictScopeManager scopeManager,
            IOpenIddictTokenManager tokenManager,
            ADTOOpenIddictClaimsPrincipalManager openIddictClaimsPrincipalManager,
            ITenantCache tenantCache,
            IExternalAuthManager externalAuthManager) : base(signInManager, userManager,
            applicationManager, authorizationManager, scopeManager, tokenManager, openIddictClaimsPrincipalManager)
        {
            _externalAuthManager = externalAuthManager;
            _tenantCache = tenantCache;
        }

        private bool ProviderKeysAreEqual(ExternalAuthenticateModel model, ExternalAuthUserInfo userInfo)
        {
            if (userInfo.ProviderKey == model.ProviderKey)
            {
                return true;
            }
            return userInfo.ProviderKey == model.ProviderKey.Replace("-", "").TrimStart('0');

        }
        private async Task<ExternalAuthUserInfo> GetExternalUserInfo(ExternalAuthenticateModel model)
        {
            var userInfo = await _externalAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);
            if (model.ProviderKey.IsNullOrWhiteSpace())
                model.ProviderKey = userInfo.ProviderKey;

            if (!ProviderKeysAreEqual(model, userInfo))
            {
                return null;
            }

            return userInfo;
        }

        private string GetTenancyNameOrNull()
        {
            if (!ADTOSharpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(ADTOSharpSession.TenantId.Value)?.TenancyName;
        }



        [HttpGet, HttpPost, Produces("application/json")]
        public override async Task<IActionResult> HandleAsync()
        {
            var request = HttpContext.GetOpenIddictServerRequest();

            if (request == null)
            {
                throw new InvalidOperationException("The OpenIDConnect request cannot retrieved!");
            }

            if (request.IsWechatGrantType())
            {
                return await HandleWechatAsync(request);
            }
            return await base.HandleAsync();
        }
    }
}
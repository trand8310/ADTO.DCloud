using ADTO.AspNetCore.OpenIddict.Claims;
using ADTOSharp;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Roles;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Extensions;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Runtime.Security;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ADTO.AspNetCore.OpenIddict.Controllers;

[Route("connect/token")]
[IgnoreAntiforgeryToken]
[ApiExplorerSettings(IgnoreApi = true)]
public partial class TokenController<TTenant, TRole, TUser> : ADTOOpenIdDictControllerBase<TTenant, TRole, TUser>
    where TTenant : ADTOSharpTenant<TUser>
    where TRole : ADTOSharpRole<TUser>, new()
    where TUser : ADTOSharpUser<TUser>
{
    public TokenController(
        ADTOSharpSignInManager<TTenant, TRole, TUser> signInManager,
        ADTOSharpUserManager<TRole, TUser> userManager,
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
        IOpenIddictTokenManager tokenManager,
        ADTOOpenIddictClaimsPrincipalManager openIddictClaimsPrincipalManager) :
        base(
            signInManager,
            userManager,
            applicationManager,
            authorizationManager,
            scopeManager,
            tokenManager,
            openIddictClaimsPrincipalManager
        )
    {
    }

    [HttpGet, HttpPost, Produces("application/json")]
    public virtual async Task<IActionResult> HandleAsync()
    {
        var request = HttpContext.GetOpenIddictServerRequest();

        if (request == null)
        {
            throw new InvalidOperationException("The OpenIDConnect request cannot retrieved!");
        }

        if (request.IsPasswordGrantType())
        {
            return await HandlePasswordAsync(request);
        }

        if (request.IsAuthorizationCodeGrantType())
        {
            return await HandleAuthorizationCodeAsync(request);
        }

        if (request.IsRefreshTokenGrantType())
        {
            return await HandleRefreshTokenAsync(request);
        }

        if (request.IsDeviceCodeGrantType())
        {
            return await HandleDeviceCodeAsync(request);
        }

        if (request.IsClientCredentialsGrantType())
        {
            return await HandleClientCredentialsAsync(request);
        }

        throw new ADTOSharpException($"The specified grant type {request.GrantType} is not implemented!");
    }

    private Guid? FindTenantId(ClaimsPrincipal principal)
    {
        Check.NotNull(principal, nameof(principal));

        var tenantIdOrNull = principal.Claims.FirstOrDefault(c => c.Type == ADTOSharpClaimTypes.TenantId);
        if (tenantIdOrNull == null || tenantIdOrNull.Value.IsNullOrWhiteSpace())
        {
            return null;
        }

        if (Guid.TryParse(tenantIdOrNull.Value, out var guid))
        {
            return guid;
        }

        return null;
    }
}
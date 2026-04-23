using ADTO.AspNetCore.OpenIddict.Claims;
using ADTOSharp.AspNetCore.Mvc.Controllers;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Roles;
using ADTOSharp.Authorization.Users;
using ADTOSharp.MultiTenancy;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace ADTO.AspNetCore.OpenIddict.Controllers;

public abstract class ADTOOpenIdDictControllerBase<TTenant, TRole, TUser> : ADTOSharpController
    where TTenant : ADTOSharpTenant<TUser>
    where TRole : ADTOSharpRole<TUser>, new()
    where TUser : ADTOSharpUser<TUser>
{
    protected readonly ADTOSharpSignInManager<TTenant, TRole, TUser> SignInManager;
    protected readonly ADTOSharpUserManager<TRole, TUser> UserManager;
    protected readonly IOpenIddictApplicationManager ApplicationManager;
    protected readonly IOpenIddictAuthorizationManager AuthorizationManager;
    protected readonly IOpenIddictScopeManager ScopeManager;
    protected readonly IOpenIddictTokenManager TokenManager;
    protected readonly ADTOOpenIddictClaimsPrincipalManager OpenIddictClaimsPrincipalManager;

    protected ADTOOpenIdDictControllerBase(
        ADTOSharpSignInManager<TTenant, TRole, TUser> signInManager,
        ADTOSharpUserManager<TRole, TUser> userManager,
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
        IOpenIddictTokenManager tokenManager,
        ADTOOpenIddictClaimsPrincipalManager openIddictClaimsPrincipalManager)
    {
        SignInManager = signInManager;
        UserManager = userManager;
        ApplicationManager = applicationManager;
        AuthorizationManager = authorizationManager;
        ScopeManager = scopeManager;
        TokenManager = tokenManager;
        OpenIddictClaimsPrincipalManager = openIddictClaimsPrincipalManager;

        // TODO@OpenIddict: Handle this !!!
 
    }

    protected virtual Task<OpenIddictRequest> GetOpenIddictServerRequestAsync(HttpContext httpContext)
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException(L("TheOpenIDConnectRequestCannotBeRetrieved"));

        return Task.FromResult(request);
    }

    protected virtual async Task<IEnumerable<string>> GetResourcesAsync(ImmutableArray<string> scopes)
    {
        var resources = new List<string>();
        if (!scopes.Any())
        {
            return resources;
        }

        await foreach (var resource in ScopeManager.ListResourcesAsync(scopes))
        {
            resources.Add(resource);
        }

        return resources;
    }

    protected virtual async Task<bool> HasFormValueAsync(string name)
    {
        if (Request.HasFormContentType)
        {
            var form = await Request.ReadFormAsync();
            if (!string.IsNullOrEmpty(form[name]))
            {
                return true;
            }
        }

        return false;
    }

    protected virtual async Task<bool> PreSignInCheckAsync(TUser user)
    {
        if (!await SignInManager.CanSignInAsync(user))
        {
            return false;
        }

        if (await UserManager.IsLockedOutAsync(user))
        {
            return false;
        }

        return true;
    }
}
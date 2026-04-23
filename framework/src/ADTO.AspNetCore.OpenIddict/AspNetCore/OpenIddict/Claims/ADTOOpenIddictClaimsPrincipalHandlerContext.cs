using System;
using System.Security.Claims;
using OpenIddict.Abstractions;

namespace ADTO.AspNetCore.OpenIddict.Claims;

public class ADTOOpenIddictClaimsPrincipalHandlerContext
{
    public IServiceProvider ScopeServiceProvider { get; }

    public OpenIddictRequest OpenIddictRequest { get; }

    public ClaimsPrincipal Principal { get; }

    public ADTOOpenIddictClaimsPrincipalHandlerContext(IServiceProvider scopeServiceProvider, OpenIddictRequest openIddictRequest, ClaimsPrincipal principal)
    {
        ScopeServiceProvider = scopeServiceProvider;
        OpenIddictRequest = openIddictRequest;
        Principal = principal;
    }
}
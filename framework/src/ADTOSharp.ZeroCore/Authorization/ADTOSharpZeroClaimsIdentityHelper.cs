using System;
using System.Security.Claims;
using ADTOSharp.Runtime.Security;

namespace ADTOSharp.Authorization;

internal static class ADTOSharpZeroClaimsIdentityHelper
{
    public static Guid? GetTenantId(ClaimsPrincipal principal)
    {
        var tenantIdOrNull = principal?.FindFirstValue(ADTOSharpClaimTypes.TenantId);
        if (tenantIdOrNull == null)
        {
            return null;
        }

        return Guid.Parse(tenantIdOrNull);
    }
}
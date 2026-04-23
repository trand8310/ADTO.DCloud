using System;
using System.Linq;
using ADTOSharp.Runtime.Security;
using Microsoft.AspNetCore.SignalR;

namespace ADTOSharp.AspNetCore.SignalR.Hubs;

public static class HubCallerContextExtensions
{
    public static Guid? GetTenantId(this HubCallerContext context)
    {
        if (context?.User == null)
        {
            return null;
        }

        var tenantIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == ADTOSharpClaimTypes.TenantId);
        if (string.IsNullOrEmpty(tenantIdClaim?.Value))
        {
            return null;
        }

        return Guid.Parse(tenantIdClaim.Value);
    }

    public static Guid? GetUserIdOrNull(this HubCallerContext context)
    {
        if (context?.User == null)
        {
            return null;
        }

        var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == ADTOSharpClaimTypes.UserId);
        if (string.IsNullOrEmpty(userIdClaim?.Value))
        {
            return null;
        }

        if (!Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return null;
        }

        return userId;
    }

    public static Guid GetUserId(this HubCallerContext context)
    {
        var userId = context.GetUserIdOrNull();
        if (userId == null)
        {
            throw new ADTOSharpException("UserId is null! Probably, user is not logged in.");
        }

        return userId.Value;
    }

    public static Guid? GetImpersonatorUserId(this HubCallerContext context)
    {
        if (context?.User == null)
        {
            return null;
        }

        var impersonatorUserIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == ADTOSharpClaimTypes.ImpersonatorUserId);
        if (string.IsNullOrEmpty(impersonatorUserIdClaim?.Value))
        {
            return null;
        }

        return Guid.Parse(impersonatorUserIdClaim.Value);
    }

    public static Guid? GetImpersonatorTenantId(this HubCallerContext context)
    {
        if (context?.User == null)
        {
            return null;
        }

        var impersonatorTenantIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == ADTOSharpClaimTypes.ImpersonatorTenantId);
        if (string.IsNullOrEmpty(impersonatorTenantIdClaim?.Value))
        {
            return null;
        }

        return Guid.Parse(impersonatorTenantIdClaim.Value);
    }

    public static UserIdentifier ToUserIdentifier(this HubCallerContext context)
    {
        var userId = context.GetUserIdOrNull();
        if (userId == null)
        {
            return null;
        }

        return new UserIdentifier(context.GetTenantId(), context.GetUserId());
    }
}
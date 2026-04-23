using System;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection;

public class ADTOSharpIdentityBuilder : IdentityBuilder
{
    public Type TenantType { get; }

    public ADTOSharpIdentityBuilder(IdentityBuilder identityBuilder, Type tenantType)
        : base(identityBuilder.UserType, identityBuilder.RoleType, identityBuilder.Services)
    {
        TenantType = tenantType;
    }
}
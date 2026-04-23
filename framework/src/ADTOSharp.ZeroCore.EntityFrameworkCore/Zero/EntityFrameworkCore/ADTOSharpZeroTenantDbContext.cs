using ADTOSharp.Authorization.Roles;
using ADTOSharp.Authorization.Users;
using ADTOSharp.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace ADTOSharp.Zero.EntityFrameworkCore;

[MultiTenancySide(MultiTenancySides.Tenant)]
public abstract class ADTOSharpZeroTenantDbContext<TRole, TUser, TSelf> : ADTOSharpZeroCommonDbContext<TRole, TUser, TSelf>
    where TRole : ADTOSharpRole<TUser>
    where TUser : ADTOSharpUser<TUser>
    where TSelf : ADTOSharpZeroTenantDbContext<TRole, TUser, TSelf>
{

    /// <summary>
    ///
    /// </summary>
    /// <param name="options"></param>
    protected ADTOSharpZeroTenantDbContext(DbContextOptions<TSelf> options)
        : base(options)
    {

    }
}
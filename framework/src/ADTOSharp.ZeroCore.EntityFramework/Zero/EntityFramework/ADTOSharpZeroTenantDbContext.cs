using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using ADTOSharp.Authorization.Roles;
using ADTOSharp.Authorization.Users;
using ADTOSharp.MultiTenancy;

namespace ADTOSharp.Zero.EntityFramework;

[MultiTenancySide(MultiTenancySides.Tenant)]
public abstract class ADTOSharpZeroTenantDbContext<TRole, TUser, TSelf> : ADTOSharpZeroCommonDbContext<TRole, TUser, TSelf>
    where TRole : ADTOSharpRole<TUser>
    where TUser : ADTOSharpUser<TUser>
    where TSelf : ADTOSharpZeroTenantDbContext<TRole, TUser, TSelf>
{

    /// <summary>
    /// Default constructor.
    /// Do not directly instantiate this class. Instead, use dependency injection!
    /// </summary>
    protected ADTOSharpZeroTenantDbContext()
    {

    }

    /// <summary>
    /// Constructor with connection string parameter.
    /// </summary>
    /// <param name="nameOrConnectionString">Connection string or a name in connection strings in configuration file</param>
    protected ADTOSharpZeroTenantDbContext(string nameOrConnectionString)
        : base(nameOrConnectionString)
    {

    }

    protected ADTOSharpZeroTenantDbContext(DbCompiledModel model)
        : base(model)
    {

    }

    /// <summary>
    /// This constructor can be used for unit tests.
    /// </summary>
    protected ADTOSharpZeroTenantDbContext(DbConnection existingConnection, bool contextOwnsConnection)
        : base(existingConnection, contextOwnsConnection)
    {

    }

    protected ADTOSharpZeroTenantDbContext(string nameOrConnectionString, DbCompiledModel model)
        : base(nameOrConnectionString, model)
    {
    }

    protected ADTOSharpZeroTenantDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
        : base(objectContext, dbContextOwnsObjectContext)
    {
    }

    protected ADTOSharpZeroTenantDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
        : base(existingConnection, model, contextOwnsConnection)
    {
    }
}
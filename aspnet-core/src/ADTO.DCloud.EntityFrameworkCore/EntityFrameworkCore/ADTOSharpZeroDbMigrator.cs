using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFrameworkCore;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Zero.EntityFrameworkCore;

namespace ADTO.DCloud.EntityFrameworkCore;

public class ADTOSharpZeroDbMigrator : ADTOSharpZeroDbMigrator<DCloudDbContext>
{
    public ADTOSharpZeroDbMigrator(
        IUnitOfWorkManager unitOfWorkManager,
        IDbPerTenantConnectionStringResolver connectionStringResolver,
        IDbContextResolver dbContextResolver)
        : base(
            unitOfWorkManager,
            connectionStringResolver,
            dbContextResolver)
    {
    }
}


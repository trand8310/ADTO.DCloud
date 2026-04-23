using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ADTOSharp.EntityFrameworkCore.Repositories;

public class EfCoreRepositoryBase<TDbContext, TEntity> : EfCoreRepositoryBase<TDbContext, TEntity, int>, IRepository<TEntity>
    where TEntity : class, IEntity<int>
    where TDbContext : DbContext
{
    public EfCoreRepositoryBase(IDbContextProvider<TDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
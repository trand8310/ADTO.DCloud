using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.EntityFrameworkCore;
using ADTOSharp.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EntityFrameworkCore.Repositories;

/// <summary>
/// Base class for custom repositories of the application.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
public abstract class DCloudRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<DCloudDbContext, TEntity, TPrimaryKey>
    where TEntity : class, IEntity<TPrimaryKey>
{
    protected DCloudRepositoryBase(IDbContextProvider<DCloudDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}

/// <summary>
/// Base class for custom repositories of the application.
/// This is a shortcut of <see cref="DCloudRepositoryBase{TEntity,TPrimaryKey}"/> for <see cref="int"/> primary key.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public abstract class DCloudRepositoryBase<TEntity> : DCloudRepositoryBase<TEntity, int>, IRepository<TEntity>
    where TEntity : class, IEntity<int>
{
    protected DCloudRepositoryBase(IDbContextProvider<DCloudDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    // Do not add any method here, add to the class above (since this inherits it)!!!
}


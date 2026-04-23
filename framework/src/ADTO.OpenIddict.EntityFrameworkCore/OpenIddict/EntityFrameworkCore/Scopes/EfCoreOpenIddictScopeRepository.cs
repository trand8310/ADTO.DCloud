using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using ADTO.OpenIddict.Scopes;
using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFrameworkCore;
using ADTOSharp.EntityFrameworkCore.Repositories;
using ADTOSharp.Extensions;
using ADTOSharp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ADTO.OpenIddict.EntityFrameworkCore.Scopes;

public class EfCoreOpenIddictScopeRepository<TDbContext> : EfCoreRepositoryBase<TDbContext, OpenIddictScope, Guid>,
    IOpenIddictScopeRepository
    where TDbContext : DbContext, IOpenIddictDbContext
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public EfCoreOpenIddictScopeRepository(
        IDbContextProvider<TDbContext> dbContextProvider,
        IUnitOfWorkManager unitOfWorkManager)
        : base(dbContextProvider)
    {
        _unitOfWorkManager = unitOfWorkManager;
    }

    protected async Task<DbSet<OpenIddictScope>> GetDbSetAsync()
    {
        return (await GetDbContextAsync()).Set<OpenIddictScope>();
    }

    public async Task<List<OpenIddictScope>> GetListAsync(string sorting, int pageNumber, int pageSize,
        string filter = null,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), x =>
                    x.Name.Contains(filter) ||
                    x.DisplayName.Contains(filter) ||
                    x.Description.Contains(filter))
                .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(OpenIddictScope.Name) : sorting)
                .PageBy(pageNumber, pageSize)
                .ToListAsync(cancellationToken);
        });
    }

    public async Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetDbSetAsync())
                .WhereIf(!filter.IsNullOrWhiteSpace(), x =>
                    x.Name.Contains(filter) ||
                    x.DisplayName.Contains(filter) ||
                    x.Description.Contains(filter))
                .LongCountAsync(cancellationToken);
        });
    }

    public virtual async Task<OpenIddictScope> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Id == id,
                cancellationToken);
        });
    }

    public virtual async Task<OpenIddictScope> FindByNameAsync(string name,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetQueryableAsync())
                .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        });
    }

    public virtual async Task<List<OpenIddictScope>> FindByNamesAsync(string[] names,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetQueryableAsync()).Where(x => names.Contains(x.Name))
                .ToListAsync(cancellationToken);
        });
    }

    public virtual async Task<List<OpenIddictScope>> FindByResourceAsync(string resource,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await (await GetQueryableAsync()).Where(x => x.Resources.Contains(resource))
                .ToListAsync(cancellationToken);
        });
    }

    public virtual async Task<List<OpenIddictScope>> ListAsync(int? count, int? offset,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        if (offset.HasValue)
        {
            query = query.Skip(offset.Value);
        }

        if (count.HasValue)
        {
            query = query.Take(count.Value);
        }

        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            return await query.ToListAsync(cancellationToken);
        });
    }
}
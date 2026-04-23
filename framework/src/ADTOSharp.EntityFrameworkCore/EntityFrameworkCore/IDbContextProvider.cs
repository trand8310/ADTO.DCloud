using System.Threading.Tasks;
using ADTOSharp.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace ADTOSharp.EntityFrameworkCore;

public interface IDbContextProvider<TDbContext>
    where TDbContext : DbContext
{
    Task<TDbContext> GetDbContextAsync();

    Task<TDbContext> GetDbContextAsync(MultiTenancySides? multiTenancySide);

    TDbContext GetDbContext();

    TDbContext GetDbContext(MultiTenancySides? multiTenancySide);
}
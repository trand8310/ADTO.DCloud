using System.Data.Common;
using System.Data.Entity;

namespace ADTOSharp.EntityFramework
{
    public interface IDbContextResolver
    {
        TDbContext Resolve<TDbContext>(string connectionString)
            where TDbContext : DbContext;

        TDbContext Resolve<TDbContext>(DbConnection existingConnection, bool contextOwnsConnection)
            where TDbContext : DbContext;
    }
}
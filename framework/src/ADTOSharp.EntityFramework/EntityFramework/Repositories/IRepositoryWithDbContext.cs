using System.Data.Entity;
using System.Threading.Tasks;

namespace ADTOSharp.EntityFramework.Repositories
{
    public interface IRepositoryWithDbContext
    {
        DbContext GetDbContext();
        Task<DbContext> GetDbContextAsync();
    }
}
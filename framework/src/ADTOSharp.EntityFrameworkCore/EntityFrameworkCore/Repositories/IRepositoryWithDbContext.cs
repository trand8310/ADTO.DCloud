using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ADTOSharp.EntityFrameworkCore.Repositories;

public interface IRepositoryWithDbContext
{
    DbContext GetDbContext();

    Task<DbContext> GetDbContextAsync();
}
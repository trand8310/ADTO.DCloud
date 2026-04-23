using ADTOSharp.Domain.Entities;

namespace ADTOSharp.Dapper.Repositories
{
    public interface IDapperRepository<TEntity> : IDapperRepository<TEntity, int> where TEntity : class, IEntity<int>
    {
    }
}

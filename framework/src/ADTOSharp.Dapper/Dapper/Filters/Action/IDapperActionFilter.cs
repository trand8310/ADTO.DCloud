using ADTOSharp.Dependency;
using ADTOSharp.Domain.Entities;

namespace ADTOSharp.Dapper.Filters.Action
{
    public interface IDapperActionFilter : ITransientDependency
    {
        void ExecuteFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;
    }
}

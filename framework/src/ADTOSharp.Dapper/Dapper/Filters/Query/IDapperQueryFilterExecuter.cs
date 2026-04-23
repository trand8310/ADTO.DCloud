using System;
using System.Linq.Expressions;
using ADTOSharp.Dapper_Extensions.Predicate;
using ADTOSharp.Domain.Entities;

namespace ADTOSharp.Dapper.Filters.Query
{
    public interface IDapperQueryFilterExecuter
    {
        IPredicate ExecuteFilter<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<TPrimaryKey>;

        PredicateGroup ExecuteFilter<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>;
    }
}

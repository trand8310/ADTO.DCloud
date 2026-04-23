
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace ADTO.DCloud.Infrastructure
{
    public interface IDapperSqlExecutor
    {
        public IEnumerable<T> Query<T>(string sql, object parameters = null);
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null);
        public IEnumerable<TEntity> GetAll<TEntity, TPrimaryKey>(object parameters = null) where TEntity : class, IEntity<TPrimaryKey>;
        public Task<IEnumerable<TEntity>> GetAllAsync<TEntity, TPrimaryKey>(object parameters = null) where TEntity : class, IEntity<TPrimaryKey>;
        public Task<TEntity> SingleAsync<TEntity, TPrimaryKey>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>;
        public Task<TEntity> SingleAsync<TEntity, TPrimaryKey>(Func<TEntity, bool> predicate, TPrimaryKey id = default) where TEntity : class, IEntity<TPrimaryKey>;




        /// <summary>
        /// 执行增删改 SQL
        /// </summary>
        public int Execute(string sql, object parameters = null);

        /// <summary>
        /// 异步执行增删改 SQL
        /// </summary>
        public Task<int> ExecuteAsync(string sql, object parameters = null);
    }
}

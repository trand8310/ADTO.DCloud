using ADTO.DCloud.EntityFrameworkCore;
using ADTO.DCloud.Infrastructure;
using ADTOSharp.Data;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFrameworkCore;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;



namespace ADTO.DCloud.Dapper
{
    public class DapperSqlExecutor : IDapperSqlExecutor, ITransientDependency
    {
        private readonly IDbContextProvider<DCloudDbContext> _dbContextProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public DapperSqlExecutor(
        IDbContextProvider<DCloudDbContext> dbContextProvider,
        IUnitOfWorkManager unitOfWorkManager
        )
        {
            _dbContextProvider = dbContextProvider;
            _unitOfWorkManager = unitOfWorkManager;
        }


        public async Task<DCloudDbContext> GetDbContextAsync()
        {
            return await _dbContextProvider.GetDbContextAsync();
        }
        public DCloudDbContext GetDbContext()
        {
            return _dbContextProvider.GetDbContext();
        }
        public virtual DbConnection GetConnection()
        {
            var dbContext = GetDbContext();
            return dbContext.Database.GetDbConnection();
        }

        public virtual async Task<DbConnection> GetConnectionAsync()
        {
            var dbContext = await GetDbContextAsync();
            return dbContext.Database.GetDbConnection();
        }
        public virtual async Task<DbTransaction> GetActiveTransactionAsync()
        {
            var dbContext = await GetDbContextAsync();
            return dbContext.Database.CurrentTransaction?.GetDbTransaction();
        }
        public virtual DbTransaction GetActiveTransaction()
        {
            var dbContext = GetDbContext();
            return dbContext.Database.CurrentTransaction?.GetDbTransaction();
        }


        public virtual int? Timeout => null;

        public IEnumerable<T> Query<T>(string sql, object parameters = null)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                var dbContext = GetDbContext();
                var connection = dbContext.Database.GetDbConnection();
                var activeTransaction = dbContext.Database.CurrentTransaction?.GetDbTransaction();
                return connection.Query<T>(sql, parameters, transaction: activeTransaction, commandTimeout: Timeout);
            });
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var dbContext = await GetDbContextAsync();
                var connection = dbContext.Database.GetDbConnection();
                var activeTransaction = dbContext.Database.CurrentTransaction?.GetDbTransaction();
                return await connection.QueryAsync<T>(sql, parameters, transaction: activeTransaction, commandTimeout: Timeout);
            });
        }

        public IEnumerable<TEntity> GetAll<TEntity, TPrimaryKey>(object parameters = null) where TEntity : class, IEntity<TPrimaryKey>
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                var query = $"SELECT * FROM [{typeof(TEntity).Name}]";
                var connection = GetConnection();
                var activeTransaction = GetActiveTransaction();
                return connection.Query<TEntity>(query, parameters, transaction: activeTransaction, commandTimeout: Timeout);
            });
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync<TEntity, TPrimaryKey>(object parameters = null) where TEntity : class, IEntity<TPrimaryKey>
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var query = $"SELECT * FROM [{typeof(TEntity).Name}]";
                var connection = await GetConnectionAsync();
                var activeTransaction = await GetActiveTransactionAsync();
                return await connection.QueryAsync<TEntity>(query, parameters, transaction: activeTransaction, commandTimeout: Timeout);
            });
        }

        public async Task<TEntity> SingleAsync<TEntity, TPrimaryKey>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>
        {
            return await SingleAsync<TEntity, TPrimaryKey>(e => e.Id.Equals(id), id);
        }

        public async Task<TEntity> SingleAsync<TEntity, TPrimaryKey>(Func<TEntity, bool> predicate, TPrimaryKey id = default) where TEntity : class, IEntity<TPrimaryKey>
        {
            var all = await GetAllAsync<TEntity, TPrimaryKey>();
            var list = all.Where(predicate).ToList();
            if (!list.Any())
                throw !Equals(id, default(TPrimaryKey)) ? new EntityNotFoundException(typeof(TEntity), id) : new EntityNotFoundException(typeof(TEntity).FullName);
            if (list.Count > 1)
                throw new InvalidOperationException("查询结果不唯一");
            return list[0];
        }

        public TEntity Single<TEntity, TPrimaryKey>(string sql, object? param = null) where TEntity : class, IEntity<TPrimaryKey>
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                var connection = GetConnection();
                var activeTransaction = GetActiveTransaction();
                return connection.QuerySingle<TEntity>(sql, param, transaction: activeTransaction, commandTimeout: Timeout);
            });
        }

        public async Task<TEntity> SingleAsync<TEntity, TPrimaryKey>(string sql, object? param = null) where TEntity : class, IEntity<TPrimaryKey>
        {
            return await _unitOfWorkManager.WithUnitOfWork(async () =>
            {
                var connection = await GetConnectionAsync();
                var activeTransaction = await GetActiveTransactionAsync();
                return await connection.QuerySingleAsync<TEntity>(sql, param, transaction: activeTransaction, commandTimeout: Timeout);
            });
        }

        public TEntity FirstOrDefault<TEntity, TPrimaryKey>(string sql, object? param = null) where TEntity : class, IEntity<TPrimaryKey>
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                var connection = GetConnection();
                var activeTransaction = GetActiveTransaction();
                return connection.QueryFirstOrDefault<TEntity>(sql, param, transaction: activeTransaction, commandTimeout: Timeout);
            });
        }
        public async Task<TEntity> FirstOrDefaultAsync<TEntity, TPrimaryKey>(string sql, object? param = null) where TEntity : class, IEntity<TPrimaryKey>
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var connection = await GetConnectionAsync();
                var activeTransaction = await GetActiveTransactionAsync();
                return await connection.QueryFirstOrDefaultAsync<TEntity>(sql, param, transaction: activeTransaction, commandTimeout: Timeout);
            });
        }

        /// <summary>
        /// 执行增删改 SQL
        /// </summary>
        public int Execute(string sql, object parameters = null)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                var connection = GetConnection();
                var activeTransaction = GetActiveTransaction();
                return connection.Execute(sql, parameters, transaction: activeTransaction, commandTimeout: Timeout);
            });
        }

        /// <summary>
        /// 异步执行增删改 SQL
        /// </summary>
        public async Task<int> ExecuteAsync(string sql, object parameters = null)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var connection = await GetConnectionAsync();
                var activeTransaction = await GetActiveTransactionAsync();
                return await connection.ExecuteAsync(sql, parameters, transaction: activeTransaction, commandTimeout: Timeout);
            });
        }




    }
}

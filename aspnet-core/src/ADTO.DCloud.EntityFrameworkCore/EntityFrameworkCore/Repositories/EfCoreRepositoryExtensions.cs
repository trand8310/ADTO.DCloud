using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.EntityFrameworkCore.Repositories;


namespace ADTO.DCloud.EntityFrameworkCore.Repositories;

public static class EfCoreRepositoryExtensions
{
    /// <summary>
    /// 同步版 批量更新
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="repository"></param>
    /// <param name="entities"></param>
    //public static void UpdateRange<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository,
    //    params TEntity[] entities)
    //    where TEntity : class, IEntity<TPrimaryKey>
    //{
    //    repository.GetDbContext().BulkUpdate(entities.ToArray<object>());
    //}
    /// <summary>
    /// 同步版,批量更新
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="repository"></param>
    /// <param name="entities"></param>
    //public static void UpdateRange<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository,
    //    IEnumerable<TEntity> entities)
    //    where TEntity : class, IEntity<TPrimaryKey>
    //{
    //    repository.GetDbContext().BulkUpdate(entities);
    //}

    /// <summary>
    /// 异步版 批量更新
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="repository"></param>
    /// <param name="entities"></param>
    /// <returns></returns>

    //public static async Task UpdateRangeAsync<TEntity, TPrimaryKey>(
    //    this IRepository<TEntity, TPrimaryKey> repository, params TEntity[] entities)
    //    where TEntity : class, IEntity<TPrimaryKey>
    //{
    //    await repository.GetDbContext().BulkUpdateAsync(entities.ToArray<object>());
    //}
    /// <summary>
    /// 异步版,批量更新
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="repository"></param>
    /// <param name="entities"></param>
    /// <returns></returns>
    //public static async Task UpdateRangeAsync<TEntity, TPrimaryKey>(
    //    this IRepository<TEntity, TPrimaryKey> repository, IEnumerable<TEntity> entities)
    //    where TEntity : class, IEntity<TPrimaryKey>
    //{
    //    await repository.GetDbContext().BulkUpdateAsync(entities);
    //}
}
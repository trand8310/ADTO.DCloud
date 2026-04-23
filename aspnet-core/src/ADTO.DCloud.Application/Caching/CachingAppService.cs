using ADTO.DCloud.Auditing.Dto;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Caching.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.Runtime.Caching.Memory;
using ADTOSharp.Runtime.Caching.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ADTO.DCloud.Caching
{
    /// <summary>
    /// “缓存管理”页面使用的应用程序服务。
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Host_Maintenance)]
    public class CachingAppService : DCloudAppServiceBase, ICachingAppService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IADTOSharpRedisCacheDatabaseProvider _redisManager;
        #endregion

        #region Ctor




        public CachingAppService(ICacheManager cacheManager, IADTOSharpRedisCacheDatabaseProvider redisManager)
        {
            _cacheManager = cacheManager;
            _redisManager = redisManager;
        }
        #endregion

        #region Utilities
        #endregion

        #region Methods
        /// <summary>
        /// 获取系统所有的缓存列表
        /// </summary>
        /// <returns></returns>
        public ListResultDto<CacheDto> GetAllCaches()
        {
            var caches = _cacheManager.GetAllCaches()
                                        .Select(cache => new CacheDto
                                        {
                                            Name = cache.Name
                                        })
                                        .ToList();

            return new ListResultDto<CacheDto>(caches);
        }

        /// <summary>
        /// 获取系统中的所有REDIS缓存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<RedisScanResultDto> GetAllRedisCaches(GetCachesDto input)
        {

            var db = _redisManager.GetDatabase();
            var redis = db.Multiplexer;

            var pattern = input.Keyword ?? "*";

            var server = redis.GetServer(redis.GetEndPoints().First());
            long cursor = input.Cursor;
            if (cursor < 0)
            {
                cursor = 0;
            }

            var scan = await server.ExecuteAsync(
                "SCAN",
                cursor.ToString(),
                "MATCH",
                pattern,
                "COUNT",
                input.PageSize.ToString()
            );

            var redisResult = (RedisResult[])scan;
            var result = new RedisScanResultDto();
            result.Cursor = int.Parse(redisResult[0].ToString());

            var keys = (RedisResult[])redisResult[1];
            foreach (var keyResult in keys)
            {
                var key = (RedisKey)keyResult.ToString();

                var item = new RedisKeyItemDto
                {
                    Key = key
                };

                var ttl = db.KeyTimeToLive(key);
                if (ttl.HasValue)
                    item.TtlSeconds = ttl.Value.TotalSeconds;

                if (input.ShowValue)
                {
                    var value = db.StringGet(key);
                    item.Value = value.HasValue ? value.ToString().Substring(0, 20) : null;
                }
                result.Items.Add(item);
            }
            return result;


        }

        /// <summary>
        /// 清除指定Key值的缓存条目
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task ClearCache(EntityDto<string> input)
        {
            var cache = _cacheManager.GetCache(input.Id);
            await cache.ClearAsync();
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task ClearAllCaches()
        {
            //if (!CanClearAllCaches())
            //{
            //    throw new ApplicationException("只有缓存使用的是内存缓存时才可以使用！");
            //}
            var caches = _cacheManager.GetAllCaches();
            foreach (var cache in caches)
            {
                await cache.ClearAsync();
            }
        }
        /// <summary>
        /// 检测,缓存类型,并判断是否可以执行清除操作
        /// 只有缓存使用的是内存缓存时才可以使用
        /// </summary>
        /// <returns></returns>
        public bool CanClearAllCaches()
        {
            return _cacheManager.GetType() == typeof(ADTOSharpMemoryCacheManager);
        }

        #endregion

    }
}

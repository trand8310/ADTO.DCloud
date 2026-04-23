using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ADTO.DCloud.Tasks.Dto;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Logging;
using ADTOSharp.Threading.BackgroundWorkers;
using ADTOSharp.Threading.Timers;
using ADTOSharp.Timing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace ADTO.DCloud.Tasks.TaskWorks
{
    /// <summary>
    /// 服务测试
    /// </summary>
    public class ExpiredTestWork : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        //private readonly IRepository<TaskScheduler, Guid> _taskRepository;
        #region
        private readonly ITaskSchedulerAppService _taskSchedulerAppService;
        private List<TaskSchedulerDto> _cachedTasks;
        private DateTime _lastCacheUpdateTime = DateTime.MinValue;
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpClientFactory _httpClientFactory;
        public ExpiredTestWork(ADTOSharpAsyncTimer timer
            //, IRepository<TaskScheduler, Guid> taskRepository
            , ITaskSchedulerAppService taskSchedulerAppService
            , IHttpClientFactory httpClientFactory
            , IDistributedCache distributedCache) : base(timer)
        {
            //_taskRepository = taskRepository;
            _taskSchedulerAppService = taskSchedulerAppService;
            _distributedCache = distributedCache;
            _httpClientFactory = httpClientFactory;
            Timer.Period = 1 * 60 * 1000;
            //应用启动时立即执行一次 然后每隔 Period 时间再执行,默认为false
            Timer.RunOnStart = true;
        }
        #endregion

        /// <summary>
        /// 服务主体方法
        /// </summary>
        /// <returns></returns>
        protected override async Task DoWorkAsync()
        {
            try
            {
                //得到系统任务表数据
                await RefreshCacheIfNeededAsync();
                var now = Clock.Now;
                foreach (var task in _cachedTasks)
                {
                    //是否满足执行条件
                    if (await ShouldRunAsync(task, now))
                    {
                        //执行具体的任务操作
                        await ExecuteTaskAsync(task, now);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取系统任务配置项
        /// </summary>
        /// <returns></returns>
        private async Task RefreshCacheIfNeededAsync()
        {
            if (_cachedTasks == null || (Clock.Now - _lastCacheUpdateTime).TotalMinutes > 5)
            {
                //_cachedTasks = await _taskRepository.GetAll().Where(p => p.State).ToListAsync();

                _cachedTasks = await _taskSchedulerAppService.GetTaskSchedulerListByState();
                _lastCacheUpdateTime = Clock.Now;
            }
        }

        /// <summary>
        /// 是否满足执行时间
        /// </summary>
        /// <param name="task"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        private async Task<bool> ShouldRunAsync(TaskSchedulerDto task, DateTime now)
        {
            try
            {
                if (string.IsNullOrEmpty(task.CycleJsonValue))
                    return false;
                //获取json 配置值 {"day":1,"hour":"2","minute":"3"}
                var config = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(task.CycleJsonValue);
                if (config == null) return false;

                return task.CycleType switch
                {
                    "EveryDay" => CheckDaily(config, now),
                    "Nsecond" => CheckSecondly(config, now),
                    "EveryMonth" => CheckMonthly(config, now),
                    "EveryWeek" => CheckWeekly(config, now),
                    "Nminute" => CheckMinutely(config, now),
                    "Nhour" => CheckHourly(config, now),
                    "EveryHour" => CheckMinutely(config, now),
                    "Nday" => await CheckIntervalDaysAsync(config, now, task),
                    _ => false
                };
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        #region 检查逻辑
        private static bool CheckDaily(Dictionary<string, JsonElement> config, DateTime now)
        {
            return now.Hour == config["hour"].GetInt32() &&
                   now.Minute == config["minute"].GetInt32();
        }

        private static bool CheckSecondly(Dictionary<string, JsonElement> config, DateTime now)
        {
            return now.Second % config["second"].GetInt32() == 0;
        }

        private static bool CheckMonthly(Dictionary<string, JsonElement> config, DateTime now)
        {
            return now.Day == config["day"].GetInt32() &&
                   now.Hour == config["hour"].GetInt32() &&
                   now.Minute == config["minute"].GetInt32();
        }

        private static bool CheckWeekly(Dictionary<string, JsonElement> config, DateTime now)
        {
            return (int)now.DayOfWeek == config["week"].GetInt32() &&
                   now.Hour == config["hour"].GetInt32() &&
                   now.Minute == config["minute"].GetInt32();
        }

        private static bool CheckMinutely(Dictionary<string, JsonElement> config, DateTime now)
        {
            return now.Minute == config["minute"].GetInt32();
        }

        private static bool CheckHourly(Dictionary<string, JsonElement> config, DateTime now)
        {
            return now.Minute == config["minute"].GetInt32() &&
                   now.Hour % config["hour"].GetInt32() == 0;
        }

        private async Task<bool> CheckIntervalDaysAsync(Dictionary<string, JsonElement> config, DateTime now, TaskSchedulerDto task)
        {
            // 使用分布式缓存获取上次执行时间
            var cacheKey = $"TaskLastRun:{task.Id}";
            var lastRunStr = await _distributedCache.GetStringAsync(cacheKey);
            var lastRun = lastRunStr != null ? DateTime.Parse(lastRunStr) : DateTime.MinValue;

            var intervalDays = config["day"].GetInt32();
            return (now.Date - lastRun.Date).TotalDays >= intervalDays &&
                   now.Hour == config["hour"].GetInt32() &&
                   now.Minute == config["minute"].GetInt32();
        }
        #endregion

        /// <summary>
        /// 执行具体任务逻辑
        /// </summary>
        /// <param name="task"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        private async Task ExecuteTaskAsync(TaskSchedulerDto task, DateTime now)
        {
            var cacheKey = $"TaskLastRun:{task.Id}";
            var lockKey = $"TaskLock:{task.Id}";

            // 获取分布式锁，防止多实例重复执行
            //await using (var handle = await _distributedCache.CreateLockAsync(lockKey, TimeSpan.FromSeconds(30)))
            //{
            //if (handle != null)
            //{
            try
            {
                // 再次检查，防止获取锁期间状态变化
                if (!await ShouldRunAsync(task, now)) return;

                // 执行HTTP请求
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(task.ExecuteName);

                if (!response.IsSuccessStatusCode)
                {
                    Logger.Warn($"任务执行失败: {task.Title}, StatusCode: {response.StatusCode}");
                }

                // 更新执行时间
                await _distributedCache.SetStringAsync(
                    cacheKey,
                    now.ToString("O"),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                    });

                // 更新数据库记录
                //task.LastModificationTime = now;
                //await _taskRepository.UpdateAsync(task);

                Logger.Info($"成功执行任务: {task.Title} [{task.CycleType}]");
            }
            catch (Exception ex)
            {
                //Logger.Error(ex, $"任务执行异常: {task.Title}");
            }
            //    }
            //}
        }
    }
}

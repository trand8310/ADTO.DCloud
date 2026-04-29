using ADTO.DCloud.Tasks.Dto;
using ADTOSharp.Threading.BackgroundWorkers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ADTO.DCloud.Tasks.TaskManage
{
    public class DynamicTaskManager : IDynamicTaskManager, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<ICycleConfigParser> _cycleParsers;
        private readonly ILogger<DynamicTaskManager> _logger;
        private readonly ConcurrentDictionary<Guid, Timer> _activeTimers = new();
        private readonly ConcurrentDictionary<Guid, DateTime> _lastScheduledTimes = new();
        private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _taskLocks = new();
        private readonly SemaphoreSlim _reloadLock = new(1, 1);
        private readonly ITaskSchedulerAppService _taskSchedulerAppService;
        public DynamicTaskManager(
            IServiceProvider serviceProvider,
            IEnumerable<ICycleConfigParser> cycleParsers,
            ILogger<DynamicTaskManager> logger
            , ITaskSchedulerAppService taskSchedulerAppService)
        {
            _serviceProvider = serviceProvider;
            _cycleParsers = cycleParsers;
            _logger = logger;
            _taskSchedulerAppService = taskSchedulerAppService;
        }

        public async Task InitializeAsync()
        {
            await ReloadTasksAsync();
        }

        public async Task ReloadTasksAsync()
        {
            await _reloadLock.WaitAsync();
            try
            {
                _logger.LogInformation("Reloading background tasks...");

                // 停止所有现有任务
                await StopAllAsync();

                // 加载所有启用的任务
                var activeTasks = await _taskSchedulerAppService.GetTaskSchedulerListByState();

                foreach (var taskConfig in activeTasks)
                {
                    try
                    {
                        await ScheduleTaskAsync(taskConfig);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to schedule task {taskConfig.Title} (ID: {taskConfig.Id})");
                    }
                }
            }
            finally
            {
                _reloadLock.Release();
            }
        }

        private async Task ScheduleTaskAsync(TaskSchedulerDto taskConfig)
        {
            var parser = _cycleParsers.FirstOrDefault(p => p.CanParse(taskConfig.CycleType));
            if (parser == null)
            {
                _logger.LogWarning($"No parser found for cycle type: {taskConfig.CycleType}");
                return;
            }

            // 计算下次执行时间
            var now = DateTime.Now;
            var lastExecutionTime = taskConfig.LastExecutionTime ?? now;
            var nextExecutionTime = parser.GetNextExecutionTime(taskConfig.CycleJsonValue, lastExecutionTime);

            // 更新数据库中的下次执行时间
            taskConfig.NextExecutionTime = nextExecutionTime;
            //_dbContext.BackgroundTasks.Update(taskConfig);
            //await _dbContext.SaveChangesAsync();
            await _taskSchedulerAppService.UpdateNextExecutionTime(taskConfig.Id, nextExecutionTime);

            // 计算初始延迟
            var initialDelay = nextExecutionTime - now;
            if (initialDelay < TimeSpan.Zero)
            {
                initialDelay = TimeSpan.Zero;
            }

            // 创建单次触发定时器，执行完成后按配置重新计算下次触发时间
            var timer = new Timer(
                async _ => await ExecuteAndRescheduleTaskAsync(taskConfig.Id),
                null,
                initialDelay,
                Timeout.InfiniteTimeSpan);

            _activeTimers[taskConfig.Id] = timer;
            _lastScheduledTimes[taskConfig.Id] = nextExecutionTime;

            _logger.LogInformation($"Scheduled task: {taskConfig.Title} (ID: {taskConfig.Id}), " +
                                 $"Next run at: {nextExecutionTime:yyyy-MM-dd HH:mm:ss}");
        }

        private async Task ExecuteTaskAsync(Guid taskId)
        {
            var taskLock = _taskLocks.GetOrAdd(taskId, _ => new SemaphoreSlim(1, 1));
            if (!await taskLock.WaitAsync(0))
            {
                _logger.LogWarning("Task {TaskId} is already running, skipping overlapping execution.", taskId);
                return;
            }

            try
            {
            using var scope = _serviceProvider.CreateScope();
            //var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            //var taskConfig = await dbContext.BackgroundTasks.FindAsync(taskId);
            var taskConfig = await _taskSchedulerAppService.GetTaskSchedulerByIdUnitOfWork(taskId);
            if (taskConfig == null || !taskConfig.State)
            {
                _logger.LogWarning($"Task ID {taskId} not found or disabled, stopping timer");
                if (_activeTimers.TryRemove(taskId, out var timer))
                {
                    timer.Dispose();
                }
                return;
            }

            var history = new TaskExecutionHistoryDto
            {
                TaskSchedulerId = taskId,
                StartTime = DateTime.Now
            };

            try
            {
                _logger.LogInformation($"Starting task: {taskConfig.Title} (ID: {taskId})");

                // 执行任务
                if (taskConfig.ExecuteName.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    await ExecuteHttpTaskAsync(taskConfig, history);
                }
                else
                {
                    await ExecuteServiceTaskAsync(taskConfig, history, scope.ServiceProvider);
                }

                history.IsSuccess = true;
                history.EndTime = DateTime.Now;
                _logger.LogInformation($"Task {taskConfig.Title} completed successfully");
            }
            catch (Exception ex)
            {
                history.IsSuccess = false;
                history.ErrorMessage = ex.ToString();
                history.EndTime = DateTime.Now;
                _logger.LogError(ex, $"Error executing task {taskConfig.Title}");
            }
            finally
            {
                //添加历史记录
                //dbContext.TaskExecutionHistories.Add(history);
                //await dbContext.SaveChangesAsync();

                await _taskSchedulerAppService.CreateTaskExecutionHistoryAsync(history);
            }
            }
            finally
            {
                taskLock.Release();
            }
        }


        private async Task ExecuteAndRescheduleTaskAsync(Guid taskId)
        {
            try
            {
                await ExecuteTaskAsync(taskId);
            }
            finally
            {
                await RescheduleTaskAsync(taskId);
            }
        }

        private async Task RescheduleTaskAsync(Guid taskId)
        {
            if (!_activeTimers.TryGetValue(taskId, out var timer))
            {
                return;
            }

            var taskConfig = await _taskSchedulerAppService.GetTaskSchedulerByIdUnitOfWork(taskId);
            if (taskConfig == null || !taskConfig.State)
            {
                if (_activeTimers.TryRemove(taskId, out var removedTimer))
                {
                    removedTimer.Dispose();
                }
                _lastScheduledTimes.TryRemove(taskId, out _);
                return;
            }

            var parser = _cycleParsers.FirstOrDefault(p => p.CanParse(taskConfig.CycleType));
            if (parser == null)
            {
                _logger.LogWarning("No parser found for cycle type: {CycleType}", taskConfig.CycleType);
                return;
            }

            var lastScheduleTime = _lastScheduledTimes.GetOrAdd(taskId, _ => DateTime.Now);
            var nextExecutionTime = parser.GetNextExecutionTime(taskConfig.CycleJsonValue, lastScheduleTime);
            var now = DateTime.Now;
            if (nextExecutionTime <= now)
            {
                nextExecutionTime = parser.GetNextExecutionTime(taskConfig.CycleJsonValue, now);
            }

            await _taskSchedulerAppService.UpdateNextExecutionTime(taskConfig.Id, nextExecutionTime);
            _lastScheduledTimes[taskId] = nextExecutionTime;

            var nextDelay = nextExecutionTime - now;
            if (nextDelay < TimeSpan.Zero)
            {
                nextDelay = TimeSpan.Zero;
            }

            timer.Change(nextDelay, Timeout.InfiniteTimeSpan);
        }
        /// <summary>
        /// 执行接口任务
        /// </summary>
        /// <param name="taskConfig"></param>
        /// <param name="history"></param>
        /// <returns></returns>
        private async Task ExecuteHttpTaskAsync(TaskSchedulerDto taskConfig, TaskExecutionHistoryDto history)
        {
            var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient();

            var response = await client.GetAsync(taskConfig.ExecuteName);
            history.Result = $"Status: {response.StatusCode}, Content: {await response.Content.ReadAsStringAsync()}";
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// 执行服务名任务
        /// </summary>
        /// <param name="taskConfig"></param>
        /// <param name="history"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>

        private async Task ExecuteServiceTaskAsync(
            TaskSchedulerDto taskConfig,
            TaskExecutionHistoryDto history,
            IServiceProvider serviceProvider)
        {
            var workerType = Type.GetType(taskConfig.ExecuteName);
            if (workerType == null)
            {
                throw new InvalidOperationException($"Worker type not found: {taskConfig.ExecuteName}");
            }

            if (serviceProvider.GetService(workerType) is not IBackgroundWorker worker)
            {
                throw new InvalidOperationException($"Worker not registered: {taskConfig.ExecuteName}");
            }

            if (worker is IAsyncBackgroundWorker asyncWorker)
            {
                history.Result = await asyncWorker.ExecuteAsync();
            }
            else
            {
                throw new InvalidOperationException($"Unsupported worker type: {worker.GetType().FullName}. Task worker must implement IAsyncBackgroundWorker.");
            }
        }

        public async Task StartAllAsync()
        {
            await ReloadTasksAsync();
        }

        public Task StopAllAsync()
        {
            foreach (var timer in _activeTimers.Values)
            {
                timer?.Dispose();
            }
            _activeTimers.Clear();
            _lastScheduledTimes.Clear();
            return Task.CompletedTask;
        }

        public async Task ExecuteTaskNowAsync(Guid taskId)
        {
            if (_activeTimers.TryGetValue(taskId, out var timer))
            {
                await ExecuteTaskAsync(taskId);
            }
            else
            {
                //var taskConfig = await _dbContext.BackgroundTasks.FindAsync(taskId);
                var taskConfig = await _taskSchedulerAppService.GetTaskSchedulerByIdUnitOfWork(taskId);
                if (taskConfig != null)
                {
                    await ExecuteTaskAsync(taskId);
                }
            }
        }

        public async Task<bool> UpdateTaskAsync(TaskSchedulerDto taskConfig)
        {
            await _reloadLock.WaitAsync();
            try
            {
                //111111111111
                //_dbContext.BackgroundTasks.Update(taskConfig);
                //await _dbContext.SaveChangesAsync();

                // 如果任务正在运行，重新调度
                if (_activeTimers.TryGetValue(taskConfig.Id, out var oldTimer))
                {
                    oldTimer.Dispose();
                    _activeTimers.TryRemove(taskConfig.Id, out _);

                    if (taskConfig.State)
                    {
                        await ScheduleTaskAsync(taskConfig);
                    }
                }
                else if (taskConfig.State)
                {
                    await ScheduleTaskAsync(taskConfig);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update task {taskConfig.Id}");
                return false;
            }
            finally
            {
                _reloadLock.Release();
            }
        }

        public void Dispose()
        {
            _reloadLock?.Dispose();
            foreach (var item in _taskLocks.Values)
            {
                item.Dispose();
            }
            StopAllAsync().GetAwaiter().GetResult();
        }
    }
}

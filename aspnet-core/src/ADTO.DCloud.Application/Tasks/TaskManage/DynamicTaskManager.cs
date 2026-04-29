using ADTO.DCloud.Tasks.Dto;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.ObjectMapping;
using ADTOSharp.Threading.BackgroundWorkers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Transactions;

namespace ADTO.DCloud.Tasks.TaskManage
{
    public class DynamicTaskManager : IDynamicTaskManager, ISingletonDependency, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<ICycleConfigParser> _cycleParsers;
        private readonly ILogger<DynamicTaskManager> _logger;
        private readonly ConcurrentDictionary<Guid, Timer> _activeTimers = new();
        private readonly ConcurrentDictionary<Guid, DateTime> _lastScheduledTimes = new();
        private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _taskLocks = new();
        private readonly SemaphoreSlim _reloadLock = new(1, 1);
        private readonly IRepository<TaskScheduler, Guid> _taskSchedulerRepository;
        private readonly IRepository<TaskExecutionHistory, Guid> _taskExecutionHistoryRepository;
        private readonly Channel<Guid> _taskQueue = Channel.CreateUnbounded<Guid>();
        private readonly CancellationTokenSource _workerCts = new();
        private readonly List<Task> _queueWorkers = new();
        private readonly int _consumerCount = Math.Max(2, Environment.ProcessorCount / 2);
        private int _workersStarted;
        private readonly IObjectMapper _objectMapper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;


        public DynamicTaskManager(
            IServiceProvider serviceProvider,
            IEnumerable<ICycleConfigParser> cycleParsers,
            IRepository<TaskScheduler, Guid> taskSchedulerRepository,
            IRepository<TaskExecutionHistory, Guid> taskExecutionHistoryRepository,
            IObjectMapper objectMapper,
            IUnitOfWorkManager unitOfWorkManager,
            ILogger<DynamicTaskManager> logger)
        {
            _serviceProvider = serviceProvider;
            _cycleParsers = cycleParsers;
            _taskSchedulerRepository = taskSchedulerRepository;
            _taskExecutionHistoryRepository = taskExecutionHistoryRepository;
            _objectMapper = objectMapper;
            _unitOfWorkManager = unitOfWorkManager;
            _logger = logger;

        }

        public async Task InitializeAsync()
        {
            InitializeQueueWorkers();
            await InitializeTasksAsync();
        }

        public async Task InitializeTasksAsync()
        {
            await _reloadLock.WaitAsync();
            try
            {
                _logger.LogInformation("Reloading background tasks...");

                // 增量加载所有启用任务（不停止其他任务，避免重载造成调度抖动）
                var activeTasks = await GetTaskSchedulerListByState();
                var activeTaskIds = activeTasks.Select(x => x.Id).ToHashSet();

                foreach (var taskConfig in activeTasks)
                {
                    try
                    {
                        await UpsertTaskScheduleAsync(taskConfig);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to schedule task {taskConfig.Title} (ID: {taskConfig.Id})");
                    }
                }

                // 清理已被禁用或删除的任务
                var obsoleteIds = _activeTimers.Keys.Where(id => !activeTaskIds.Contains(id)).ToList();
                foreach (var obsoleteId in obsoleteIds)
                {
                    RemoveTaskTimer(obsoleteId);
                }
            }
            finally
            {
                _reloadLock.Release();
            }
        }


        private void InitializeQueueWorkers()
        {
            if (Interlocked.Exchange(ref _workersStarted, 1) == 1)
            {
                return;
            }

            for (var i = 0; i < _consumerCount; i++)
            {
                _queueWorkers.Add(Task.Run(() => ConsumeQueueAsync(_workerCts.Token)));
            }

            _logger.LogInformation("Started {Count} scheduler consumers.", _consumerCount);
        }

        private async Task ConsumeQueueAsync(CancellationToken cancellationToken)
        {
            await foreach (var taskId in _taskQueue.Reader.ReadAllAsync(cancellationToken))
            {
                try
                {
                    await ExecuteTaskAsync(taskId);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Task consumer failed for task {TaskId}", taskId);
                }
                finally
                {
                    await RescheduleTaskAsync(taskId);
                }
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
            await UpdateNextExecutionTime(taskConfig.Id, nextExecutionTime);

            // 计算初始延迟
            var initialDelay = nextExecutionTime - now;
            if (initialDelay < TimeSpan.Zero)
            {
                initialDelay = TimeSpan.Zero;
            }

            // 创建单次触发定时器，执行完成后按配置重新计算下次触发时间
            var timer = new Timer(
                _ => EnqueueTask(taskConfig.Id),
                null,
                initialDelay,
                Timeout.InfiniteTimeSpan);

            _activeTimers[taskConfig.Id] = timer;
            _lastScheduledTimes[taskConfig.Id] = nextExecutionTime;

            _logger.LogInformation($"Scheduled task: {taskConfig.Title} (ID: {taskConfig.Id}), " +
                                 $"Next run at: {nextExecutionTime:yyyy-MM-dd HH:mm:ss}");
        }


        private async Task UpsertTaskScheduleAsync(TaskSchedulerDto taskConfig)
        {
            RemoveTaskTimer(taskConfig.Id);
            await ScheduleTaskAsync(taskConfig);
        }

        private void RemoveTaskTimer(Guid taskId)
        {
            if (_activeTimers.TryRemove(taskId, out var timer))
            {
                timer.Dispose();
            }

            _lastScheduledTimes.TryRemove(taskId, out _);
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
                var taskConfig = await GetTaskSchedulerByIdUnitOfWork(taskId);
                if (taskConfig == null || !taskConfig.State)
                {
                    _logger.LogWarning($"Task ID {taskId} not found or disabled, stopping timer");
                    RemoveTaskTimer(taskId);
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
                        //await ExecuteServiceTaskAsync(taskConfig, history, scope.ServiceProvider);
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
                    await CreateTaskExecutionHistoryAsync(history);
                }
            }
            finally
            {
                taskLock.Release();
            }
        }


        private void EnqueueTask(Guid taskId)
        {
            if (!_taskQueue.Writer.TryWrite(taskId))
            {
                _logger.LogWarning("Failed to enqueue task: {TaskId}", taskId);
            }
        }

        private async Task RescheduleTaskAsync(Guid taskId)
        {
            if (!_activeTimers.TryGetValue(taskId, out var timer))
            {
                return;
            }

            var taskConfig = await GetTaskSchedulerByIdUnitOfWork(taskId);
            if (taskConfig == null || !taskConfig.State)
            {
                RemoveTaskTimer(taskId);
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

            await UpdateNextExecutionTime(taskConfig.Id, nextExecutionTime);
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
            TaskExecutionHistoryDto history)
        {
            //var workerType = Type.GetType(taskConfig.ExecuteName);
            //if (workerType == null)
            //{
            //    throw new InvalidOperationException($"Worker type not found: {taskConfig.ExecuteName}");
            //}

            ////if (serviceProvider.GetService(workerType) is not IBackgroundWorker worker)
            ////{
            ////    throw new InvalidOperationException($"Worker not registered: {taskConfig.ExecuteName}");
            ////}

            //if (worker is IAsyncBackgroundWorker asyncWorker)
            //{
            //    history.Result = await asyncWorker.ExecuteAsync();
            //}
            //else
            //{
            //    throw new InvalidOperationException($"Unsupported worker type: {worker.GetType().FullName}. Task worker must implement IAsyncBackgroundWorker.");
            //}
        }

        public async Task StartAllAsync()
        {
            await InitializeTasksAsync();
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
                var taskConfig = await GetTaskSchedulerByIdUnitOfWork(taskId);
                if (taskConfig != null)
                {
                    await ExecuteTaskAsync(taskId);
                }
            }
        }


        public Task RemoveTaskAsync(Guid taskId)
        {
            RemoveTaskTimer(taskId);
            return Task.CompletedTask;
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
                if (!taskConfig.State)
                {
                    RemoveTaskTimer(taskConfig.Id);
                    return true;
                }

                await UpsertTaskScheduleAsync(taskConfig);

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


        /// <summary>
        /// 获取指定任务
        /// </summary>
        /// <returns></returns>
        public async Task<TaskSchedulerDto> GetTaskSchedulerByIdUnitOfWork(Guid Id)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var info = await _taskSchedulerRepository.GetAsync(Id);
                return _objectMapper.Map<TaskSchedulerDto>(info);
            });

        }

        /// <summary>
        /// 添加系统任务历史记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateTaskExecutionHistoryAsync(TaskExecutionHistoryDto input)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var info = _objectMapper.Map<TaskExecutionHistory>(input);
                await _taskExecutionHistoryRepository.InsertAsync(info);
            });

        }


        /// <summary>
        /// 修改下次执行时间
        /// </summary>
        /// <returns></returns>
        //[UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
        public async Task UpdateNextExecutionTime(Guid Id, DateTime NextExecutionTime)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var list = await this._taskSchedulerRepository.GetAll().Where(p => p.State).ToListAsync();
                await this._taskSchedulerRepository.UpdateAsync(Id, async entity => { entity.NextExecutionTime = NextExecutionTime; });
            });
        }

        /// <summary>
        /// 获取启用状态的任务列表
        /// </summary>
        /// <returns></returns>
        //[UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
        public async Task<List<TaskSchedulerDto>> GetTaskSchedulerListByState()
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var list = await this._taskSchedulerRepository.GetAll().Where(p => p.State).ToListAsync();
                return _objectMapper.Map<List<TaskSchedulerDto>>(list);
            });
        }

        public void Dispose()
        {
            _workerCts.Cancel();
            _taskQueue.Writer.TryComplete();
            Task.WhenAll(_queueWorkers).GetAwaiter().GetResult();
            _workerCts.Dispose();
            _reloadLock?.Dispose();
            foreach (var item in _taskLocks.Values)
            {
                item.Dispose();
            }
            StopAllAsync().GetAwaiter().GetResult();
        }
    }
}

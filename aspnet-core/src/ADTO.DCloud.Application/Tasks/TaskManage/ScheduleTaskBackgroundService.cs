using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ADTO.DCloud.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ADTO.DCloud.Tasks.TaskManage;

/// <summary>
/// 基于配置中心任务表的动态调度后台服务。
/// 任务来源于 ITaskSchedulerAppService，由 IDynamicTaskManager 负责具体定时器编排与执行。
/// </summary>
public class ScheduleTaskBackgroundService : BackgroundService
{
    private readonly IDynamicTaskManager _taskManager;
    private readonly ITaskSchedulerAppService _taskSchedulerAppService;
    private readonly ILogger<ScheduleTaskBackgroundService> _logger;

    private readonly TimeSpan _scanInterval = TimeSpan.FromSeconds(15);
    private readonly TimeSpan _reloadCooldown = TimeSpan.FromSeconds(10);
    private string _lastTaskFingerprint = string.Empty;
    private int _reloadRunning;
    private DateTime _lastReloadAtUtc = DateTime.MinValue;

    public ScheduleTaskBackgroundService(
        IDynamicTaskManager taskManager,
        ITaskSchedulerAppService taskSchedulerAppService,
        ILogger<ScheduleTaskBackgroundService> logger)
    {
        _taskManager = taskManager;
        _taskSchedulerAppService = taskSchedulerAppService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ScheduleTaskBackgroundService started.");

        await _taskManager.InitializeAsync();
        _lastTaskFingerprint = await BuildFingerprintAsync(stoppingToken);

        using var timer = new PeriodicTimer(_scanInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                var currentFingerprint = await BuildFingerprintAsync(stoppingToken);
                if (!string.Equals(currentFingerprint, _lastTaskFingerprint, StringComparison.Ordinal))
                {
                    _lastTaskFingerprint = currentFingerprint;
                    await TryReloadAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ScheduleTaskBackgroundService loop failed, retrying next interval.");
            }
        }

        await _taskManager.StopAllAsync();
        _logger.LogInformation("ScheduleTaskBackgroundService stopped.");
    }

    private async Task TryReloadAsync(CancellationToken stoppingToken)
    {
        var nowUtc = DateTime.UtcNow;
        if (nowUtc - _lastReloadAtUtc < _reloadCooldown)
        {
            _logger.LogDebug("Task config changed but reload is in cooldown window, skip this round.");
            return;
        }

        if (Interlocked.CompareExchange(ref _reloadRunning, 1, 0) != 0)
        {
            _logger.LogDebug("Task reload is already running, skip duplicated trigger.");
            return;
        }

        try
        {
            _logger.LogInformation("Task configuration changed, reloading schedules.");
            await _taskManager.ReloadTasksAsync();
            _lastReloadAtUtc = DateTime.UtcNow;
        }
        finally
        {
            Interlocked.Exchange(ref _reloadRunning, 0);
        }
    }

    private async Task<string> BuildFingerprintAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var activeTasks = await _taskSchedulerAppService.GetTaskSchedulerListByState();
        return string.Join("|", activeTasks
            .OrderBy(x => x.Id)
            .Select(x => $"{x.Id}:{x.State}:{x.CycleType}:{x.CycleJsonValue}:{x.ExecuteName}:{x.NextExecutionTime:O}"));
    }
}

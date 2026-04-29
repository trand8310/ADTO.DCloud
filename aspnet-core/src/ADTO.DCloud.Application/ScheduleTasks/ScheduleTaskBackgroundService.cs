using System;
using System.Threading;
using System.Threading.Tasks;
using ADTO.DCloud.Tasks.TaskManage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ADTO.DCloud.Tasks.ScheduleTasks;

/// <summary>
/// 基于配置中心任务表的动态调度后台服务。
/// 任务来源于 ITaskSchedulerAppService，由 IDynamicTaskManager 负责具体定时器编排与执行。
/// </summary>
public class ScheduleTaskBackgroundService : BackgroundService
{
    private readonly IDynamicTaskManager _taskManager;
    private readonly ILogger<ScheduleTaskBackgroundService> _logger;


    public ScheduleTaskBackgroundService(
        IDynamicTaskManager taskManager,
        ILogger<ScheduleTaskBackgroundService> logger)
    {
        _taskManager = taskManager;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ScheduleTaskBackgroundService started.");

        // 仅在系统启动时初始化调度，避免周期性全量重载影响并行任务稳定性
        await _taskManager.InitializeAsync();

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // ignored
        }

        await _taskManager.StopAllAsync();
        _logger.LogInformation("ScheduleTaskBackgroundService stopped.");
    }



}

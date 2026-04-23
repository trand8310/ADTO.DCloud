using System.Threading.Tasks;
using ADTOSharp.Application.Services;
using ADTO.DCloud.Tasks.Dto;
using System.Collections.Generic;
using System;

namespace ADTO.DCloud.Tasks;

public interface ITaskSchedulerAppService : IApplicationService
{
    /// <summary>
    /// 获取启用状态的任务列表
    /// </summary>
    /// <returns></returns>
    Task<List<TaskSchedulerDto>> GetTaskSchedulerListByState();

    /// <summary>
    /// 修改下次执行时间
    /// </summary>
    /// <returns></returns>
    Task UpdateNextExecutionTime(Guid Id, DateTime NextExecutionTime);

    /// <summary>
    /// 修改最后执行时间
    /// </summary>
    /// <returns></returns>

    Task UpdateLastExecutionTime(Guid Id, DateTime LastExecutionTime);

    Task<TaskSchedulerDto> GetTaskSchedulerByIdUnitOfWork(Guid Id);

    /// <summary>
    /// 添加系统任务历史记录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task CreateTaskExecutionHistoryAsync(TaskExecutionHistoryDto input);
}

using ADTO.DCloud.Tasks.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADTO.DCloud.Tasks.TaskManage
{
    public interface  IDynamicTaskManager
    {
        Task InitializeAsync();
        Task InitializeTasksAsync();
        Task StartAllAsync();
        Task StopAllAsync();
        Task ExecuteTaskNowAsync(Guid taskId);
        Task<bool> UpdateTaskAsync(TaskSchedulerDto taskConfig);
        Task RemoveTaskAsync(Guid taskId);


        Task<TaskSchedulerDto> GetTaskSchedulerByIdUnitOfWork(Guid Id);
        Task CreateTaskExecutionHistoryAsync(TaskExecutionHistoryDto input);
        Task<List<TaskSchedulerDto>> GetTaskSchedulerListByState();
        Task UpdateNextExecutionTime(Guid Id, DateTime NextExecutionTime);
    }
}

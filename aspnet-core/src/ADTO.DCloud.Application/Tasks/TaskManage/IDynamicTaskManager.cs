using ADTO.DCloud.Tasks.Dto;
using System;
using System.Threading.Tasks;

namespace ADTO.DCloud.Tasks.TaskManage
{
    public interface  IDynamicTaskManager
    {
        Task InitializeAsync();
        Task ReloadTasksAsync();
        Task StartAllAsync();
        Task StopAllAsync();
        Task ExecuteTaskNowAsync(Guid taskId);
        Task<bool> UpdateTaskAsync(TaskSchedulerDto taskConfig);
    }
}

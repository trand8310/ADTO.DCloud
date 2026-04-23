using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ADTO.DCloud.Tasks.TaskManage
{
    public class DynamicTaskBackgroundService : BackgroundService
    {
        private readonly IDynamicTaskManager _taskManager;
        private readonly ILogger<DynamicTaskBackgroundService> _logger;
        private readonly TimeSpan _refreshInterval = TimeSpan.FromMinutes(5);

        public DynamicTaskBackgroundService(
            IDynamicTaskManager taskManager,
            ILogger<DynamicTaskBackgroundService> logger)
        {
            _taskManager = taskManager;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Dynamic Task Background Service is starting.");

            // 初始加载任务
            await _taskManager.InitializeAsync();

            // 定期检查任务配置变更
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_refreshInterval, stoppingToken);

                try
                {
                    await _taskManager.ReloadTasksAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reloading tasks");
                }
            }

            _logger.LogInformation("Dynamic Task Background Service is stopping.");
            await _taskManager.StopAllAsync();
        }
    }
}

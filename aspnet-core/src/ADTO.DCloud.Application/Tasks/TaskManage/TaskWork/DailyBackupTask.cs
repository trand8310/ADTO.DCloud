using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ADTO.DCloud.Tasks.TaskManage.TaskWork
{
    public class DailyBackupTask : IAsyncBackgroundWorker
    {
        private readonly ILogger<DailyBackupTask> _logger;

        public DailyBackupTask(
            ILogger<DailyBackupTask> logger
           )
        {
            _logger = logger;
        }

        public async Task<string> ExecuteAsync()
        {
            _logger.LogInformation("Starting daily backup...");

            // 模拟备份操作
            await Task.Delay(1000);
            var backupInfo = $"Backup completed at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            _logger.LogInformation(backupInfo);
            return backupInfo;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void WaitToStop()
        {
            throw new NotImplementedException();
        }
    }
}

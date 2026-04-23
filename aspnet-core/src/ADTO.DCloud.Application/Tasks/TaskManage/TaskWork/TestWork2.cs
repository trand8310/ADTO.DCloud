using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Tasks.TaskManage.TaskWork
{
    public class TestWork2 : IAsyncBackgroundWorker
    {
        private readonly ILogger<TestWork2> _logger;

        public TestWork2(
            ILogger<TestWork2> logger
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

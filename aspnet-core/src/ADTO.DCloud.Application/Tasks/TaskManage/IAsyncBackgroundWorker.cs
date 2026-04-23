using ADTOSharp.Threading.BackgroundWorkers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Tasks.TaskManage
{
    public interface IAsyncBackgroundWorker : IBackgroundWorker
    {
        Task<string> ExecuteAsync();
    }
}

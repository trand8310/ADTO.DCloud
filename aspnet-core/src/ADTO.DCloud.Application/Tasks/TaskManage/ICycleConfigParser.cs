using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Tasks.TaskManage
{
    public interface ICycleConfigParser
    {
        bool CanParse(string cycleType);
        TimeSpan GetInterval(string cycleJsonValue);
        DateTime GetNextExecutionTime(string cycleJsonValue, DateTime lastExecutionTime);
    }
}

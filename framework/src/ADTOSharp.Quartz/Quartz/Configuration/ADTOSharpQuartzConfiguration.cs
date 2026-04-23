using Quartz;
using Quartz.Impl;

namespace ADTOSharp.Quartz.Configuration
{
    public class ADTOSharpQuartzConfiguration : IADTOSharpQuartzConfiguration
    {
        public IScheduler Scheduler => StdSchedulerFactory.GetDefaultScheduler().Result;
    }
}
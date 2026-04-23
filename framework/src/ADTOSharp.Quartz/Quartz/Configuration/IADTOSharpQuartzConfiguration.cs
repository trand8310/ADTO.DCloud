using Quartz;

namespace ADTOSharp.Quartz.Configuration
{
    public interface IADTOSharpQuartzConfiguration
    {
        IScheduler Scheduler { get;}
    }
}
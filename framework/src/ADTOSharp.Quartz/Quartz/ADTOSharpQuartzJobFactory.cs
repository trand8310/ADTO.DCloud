using ADTOSharp.Dependency;
using ADTOSharp.Extensions;
using Quartz;
using Quartz.Spi;

namespace ADTOSharp.Quartz
{
    public class ADTOSharpQuartzJobFactory : IJobFactory
    {
        private readonly IIocResolver _iocResolver;

        public ADTOSharpQuartzJobFactory(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _iocResolver.Resolve(bundle.JobDetail.JobType).As<IJob>();
        }

        public void ReturnJob(IJob job)
        {
            _iocResolver.Release(job);
        }
    }
}

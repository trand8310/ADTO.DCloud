using System.Reflection;
using ADTOSharp.Dependency;
using ADTOSharp.Modules;
using ADTOSharp.Quartz.Configuration;
using ADTOSharp.Threading;
using ADTOSharp.Threading.BackgroundWorkers;
using Quartz;

namespace ADTOSharp.Quartz
{
    [DependsOn(typeof (ADTOSharpKernelModule))]
    public class ADTOSharpQuartzModule : ADTOSharpModule
    {
        private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();
        
        public override void PreInitialize()
        {
            IocManager.Register<IADTOSharpQuartzConfiguration, ADTOSharpQuartzConfiguration>();

            OneTimeRunner.Run(() =>
            {
                Configuration.Modules.ADTOSharpQuartz().Scheduler.JobFactory = new ADTOSharpQuartzJobFactory(IocManager); 
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.RegisterIfNot<IJobListener, ADTOSharpQuartzJobListener>();

            Configuration.Modules.ADTOSharpQuartz().Scheduler.ListenerManager.AddJobListener(IocManager.Resolve<IJobListener>());

            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                IocManager.Resolve<IBackgroundWorkerManager>().Add(IocManager.Resolve<IQuartzScheduleJobManager>());
            }
        }
    }
}

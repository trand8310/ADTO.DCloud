using System;
using System.Threading.Tasks;
using ADTOSharp.BackgroundJobs;
using ADTOSharp.Dependency;
using ADTOSharp.Quartz.Configuration;
using ADTOSharp.Threading.BackgroundWorkers;
using Quartz;

namespace ADTOSharp.Quartz
{
    public class QuartzScheduleJobManager : BackgroundWorkerBase, IQuartzScheduleJobManager, ISingletonDependency
    {
        private readonly IBackgroundJobConfiguration _backgroundJobConfiguration;
        private readonly IADTOSharpQuartzConfiguration _quartzConfiguration;

        public QuartzScheduleJobManager(
            IADTOSharpQuartzConfiguration quartzConfiguration,
            IBackgroundJobConfiguration backgroundJobConfiguration)
        {
            _quartzConfiguration = quartzConfiguration;
            _backgroundJobConfiguration = backgroundJobConfiguration;
        }

        public async Task ScheduleAsync<TJob>(Action<JobBuilder> configureJob, Action<TriggerBuilder> configureTrigger)
            where TJob : IJob
        {
            var jobToBuild = JobBuilder.Create<TJob>();
            configureJob(jobToBuild);
            var job = jobToBuild.Build();

            var triggerToBuild = TriggerBuilder.Create();
            configureTrigger(triggerToBuild);
            var trigger = triggerToBuild.Build();

            await _quartzConfiguration.Scheduler.ScheduleJob(job, trigger);
        }

        public async Task RescheduleAsync(TriggerKey triggerKey, Action<TriggerBuilder> configureTrigger)
        {
            var triggerToBuild = TriggerBuilder.Create();
            configureTrigger(triggerToBuild);
            var trigger = triggerToBuild.Build();

            await _quartzConfiguration.Scheduler.RescheduleJob(triggerKey, trigger);
        }

        public async Task UnscheduleAsync(TriggerKey triggerKey)
        {
            await _quartzConfiguration.Scheduler.UnscheduleJob(triggerKey);
        }

        public override void Start()
        {
            base.Start();

            if (_backgroundJobConfiguration.IsJobExecutionEnabled)
            {
                _quartzConfiguration.Scheduler.Start();
            }

            Logger.Info("Started QuartzScheduleJobManager");
        }

        public override void WaitToStop()
        {
            if (_quartzConfiguration.Scheduler != null)
            {
                try
                {
                    _quartzConfiguration.Scheduler.Shutdown(true);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.ToString(), ex);
                }
            }

            base.WaitToStop();

            Logger.Info("Stopped QuartzScheduleJobManager");
        }
    }
}
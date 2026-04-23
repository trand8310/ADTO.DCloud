using ADTOSharp.BackgroundJobs;
using ADTOSharp.Dependency;
using ADTOSharp.Events.Bus;
using ADTOSharp.Events.Bus.Exceptions;
using Hangfire.Common;
using Hangfire.Server;

namespace ADTOSharp.Hangfire
{
    public class ADTOSharpHangfireJobExceptionFilter : JobFilterAttribute, IServerFilter, ITransientDependency
    {
        public IEventBus EventBus { get; set; }

        public ADTOSharpHangfireJobExceptionFilter()
        {
            EventBus = NullEventBus.Instance;
        }

        public void OnPerforming(PerformingContext filterContext)
        {
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                EventBus.Trigger(
                    this,
                    new ADTOSharpHandledExceptionData(
                        new BackgroundJobException(
                            "A background job execution is failed on Hangfire. See inner exception for details. Use JobObject to get Hangfire background job object.",
                            filterContext.Exception
                        )
                        {
                            JobObject = filterContext.BackgroundJob
                        }
                    )
                );
            }
        }
    }
}

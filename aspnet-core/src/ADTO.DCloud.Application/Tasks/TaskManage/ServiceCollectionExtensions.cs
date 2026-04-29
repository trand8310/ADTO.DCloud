using ADTO.DCloud.ScheduleTasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ADTO.DCloud.Tasks.TaskManage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDynamicTaskScheduler(this IServiceCollection services)
        {

            // 注册周期解析器
            services.TryAddEnumerable(new[]
            {
                ServiceDescriptor.Singleton<ICycleConfigParser, EveryDayParser>(),
                ServiceDescriptor.Singleton<ICycleConfigParser, NDayParser>(),
                ServiceDescriptor.Singleton<ICycleConfigParser, EveryHourParser>(),

                ServiceDescriptor.Singleton<ICycleConfigParser, NHourParser>(),
                ServiceDescriptor.Singleton<ICycleConfigParser, NMinuteParser>(),
                ServiceDescriptor.Singleton<ICycleConfigParser, EveryWeekParser>(),
                ServiceDescriptor.Singleton<ICycleConfigParser, EveryMonthParser>(),
                ServiceDescriptor.Singleton<ICycleConfigParser, NSecondParser>(),
                

            });

            //// 注册任务管理器
            services.AddSingleton<IDynamicTaskManager, DynamicTaskManager>();
       

            //// 注册后台服务
            services.AddHostedService<ScheduleTaskBackgroundService>();

            services.AddHttpClient();
            return services;
        }
    }
}

using ADTOSharp.Hangfire.Configuration;
using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;
using Hangfire;

namespace ADTOSharp.Hangfire
{
    [DependsOn(typeof(ADTOSharpKernelModule))]
    public class ADTOSharpHangfireModule : ADTOSharpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IADTOSharpHangfireConfiguration, ADTOSharpHangfireConfiguration>();
            
            Configuration.Modules
                .ADTOSharpHangfire()
                .GlobalConfiguration
                .UseActivator(new HangfireIocJobActivator(IocManager));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ADTOSharpHangfireModule).GetAssembly());
            GlobalJobFilters.Filters.Add(IocManager.Resolve<ADTOSharpHangfireJobExceptionFilter>());
        }
    }
}

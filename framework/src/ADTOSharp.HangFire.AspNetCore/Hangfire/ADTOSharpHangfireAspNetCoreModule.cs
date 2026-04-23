using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;

namespace ADTOSharp.Hangfire
{
    [DependsOn(typeof(ADTOSharpKernelModule))]
    public class ADTOSharpHangfireAspNetCoreModule : ADTOSharpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ADTOSharpHangfireAspNetCoreModule).GetAssembly());
        }
    }
}

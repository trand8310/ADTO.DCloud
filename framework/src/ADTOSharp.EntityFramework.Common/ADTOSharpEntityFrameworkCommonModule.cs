using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;

namespace ADTOSharp.EntityFramework
{
    [DependsOn(typeof(ADTOSharpKernelModule))]
    public class ADTOSharpEntityFrameworkCommonModule : ADTOSharpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ADTOSharpEntityFrameworkCommonModule).GetAssembly());
        }
    }
}

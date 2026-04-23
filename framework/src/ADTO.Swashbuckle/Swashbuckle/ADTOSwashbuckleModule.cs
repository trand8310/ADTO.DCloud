using ADTOSharp.AspNetCore;
using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;

namespace ADTO.Swashbuckle;

[DependsOn(
    typeof(ADTOSharpAspNetCoreModule))]
public class ADTOSwashbuckleModule : ADTOSharpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(ADTOSwashbuckleModule).GetAssembly());
    }
}

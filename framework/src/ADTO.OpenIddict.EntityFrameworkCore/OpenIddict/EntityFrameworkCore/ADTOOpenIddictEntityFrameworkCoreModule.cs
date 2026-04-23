
using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;
using ADTOSharp.Zero.EntityFrameworkCore;

namespace ADTO.OpenIddict.EntityFrameworkCore;

[DependsOn(typeof(ADTOOpenIddictModule), typeof(ADTOSharpZeroCoreEntityFrameworkCoreModule))]
public class ADTOOpenIddictEntityFrameworkCoreModule : ADTOSharpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(
            typeof(ADTOOpenIddictEntityFrameworkCoreModule).GetAssembly()
        );
    }
}
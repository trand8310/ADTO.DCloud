using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;
using ADTOSharp.Zero;

namespace ADTO.OpenIddict;

[DependsOn(typeof(ADTOSharpZeroCoreModule))]
public class ADTOOpenIddictModule : ADTOSharpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(ADTOOpenIddictModule).GetAssembly());
    }
}
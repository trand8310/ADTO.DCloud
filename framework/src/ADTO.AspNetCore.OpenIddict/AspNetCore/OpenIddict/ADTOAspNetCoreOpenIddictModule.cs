using ADTO.OpenIddict;
using ADTOSharp.AspNetCore;
using ADTOSharp.Modules;
using System.Reflection;

namespace ADTO.AspNetCore.OpenIddict;

[DependsOn(typeof(ADTOSharpAspNetCoreModule), typeof(ADTOOpenIddictModule))]
public class ADTOAspNetCoreOpenIddictModule : ADTOSharpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
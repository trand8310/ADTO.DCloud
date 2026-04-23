using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;

namespace ADTOSharp.Runtime.Caching.Redis;

[DependsOn(typeof(ADTOSharpRedisCacheModule))]
public class ADTOSharpAspNetCorePerRequestRedisCacheModule : ADTOSharpModule
{
    public override void PreInitialize()
    {
        IocManager.Register<IADTOSharpPerRequestRedisCacheManager, ADTOSharpPerRequestRedisCacheManager>();
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(ADTOSharpAspNetCorePerRequestRedisCacheModule).GetAssembly());
    }
}
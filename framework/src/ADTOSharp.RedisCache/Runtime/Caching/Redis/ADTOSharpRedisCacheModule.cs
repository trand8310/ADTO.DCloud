using System.Reflection;
using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;

namespace ADTOSharp.Runtime.Caching.Redis
{
    /// <summary>
    /// This modules is used to replace ADTO's cache system with Redis server.
    /// </summary>
    [DependsOn(typeof(ADTOSharpKernelModule))]
    public class ADTOSharpRedisCacheModule : ADTOSharpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<ADTOSharpRedisCacheOptions>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ADTOSharpRedisCacheModule).GetAssembly());
        }
    }
}

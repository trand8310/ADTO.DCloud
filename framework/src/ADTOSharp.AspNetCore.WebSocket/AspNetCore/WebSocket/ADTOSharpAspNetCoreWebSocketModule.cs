using System.Reflection;
using ADTOSharp.Modules;

namespace ADTOSharp.AspNetCore.WebSocket;

[DependsOn(typeof(ADTOSharpKernelModule))]
public class ADTOSharpAspNetCoreWebSocketModule : ADTOSharpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}

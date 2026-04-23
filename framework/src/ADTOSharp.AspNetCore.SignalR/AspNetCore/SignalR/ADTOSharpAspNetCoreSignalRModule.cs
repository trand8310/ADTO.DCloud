using System.Reflection;
using ADTOSharp.AspNetCore.SignalR.Notifications;
using ADTOSharp.Modules;

namespace ADTOSharp.AspNetCore.SignalR;

/// <summary>
/// ADTO ASP.NET Core SignalR integration module.
/// </summary>
[DependsOn(typeof(ADTOSharpKernelModule))]
public class ADTOSharpAspNetCoreSignalRModule : ADTOSharpModule
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

        Configuration.Notifications.Notifiers.Add<SignalRRealTimeNotifier>();
    }
}
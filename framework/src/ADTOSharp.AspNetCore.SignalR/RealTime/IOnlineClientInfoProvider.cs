using ADTOSharp.Dependency;
using Microsoft.AspNetCore.SignalR;

namespace ADTOSharp.RealTime;

public interface IOnlineClientInfoProvider : ITransientDependency
{
    IOnlineClient CreateClientForCurrentConnection(HubCallerContext context);
}
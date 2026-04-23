using ADTOSharp.Auditing;
using ADTOSharp.RealTime;

namespace ADTOSharp.AspNetCore.SignalR.Hubs;

public class ADTOSharpCommonHub : OnlineClientHubBase
{
    public ADTOSharpCommonHub(IOnlineClientManager onlineClientManager, IOnlineClientInfoProvider clientInfoProvider)
        : base(onlineClientManager, clientInfoProvider)
    {
    }

    public void Register()
    {
        Logger.Debug("A client is registered: " + Context.ConnectionId);
    }
}
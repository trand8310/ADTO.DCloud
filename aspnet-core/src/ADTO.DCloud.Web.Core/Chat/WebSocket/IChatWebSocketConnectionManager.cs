using System.Net.WebSockets;
using ADTOSharp;

namespace ADTO.DCloud.Web.Chat.WebSocket;

public interface IChatWebSocketConnectionManager
{
    Task<string> AddConnectionAsync(WebSocket socket, UserIdentifier user);
    Task RemoveConnectionAsync(string connectionId);
    Task JoinGroupAsync(string connectionId, string groupName);
    Task LeaveGroupAsync(string connectionId, string groupName);
    Task SendAsync(UserIdentifier user, string method, object payload);
    Task SendGroupAsync(string groupName, string method, object payload);
    Task SendAllAsync(string method, object payload);
}

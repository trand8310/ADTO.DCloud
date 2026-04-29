using System.Net.WebSockets;
using System.Threading.Tasks;
using ADTOSharp;

namespace ADTO.DCloud.Notifications.WS;

public interface INotificationWebSocketConnectionManager
{
    Task<string> AddConnectionAsync(WebSocket socket, UserIdentifier user);
    Task RemoveConnectionAsync(string connectionId);
    Task SendAsync(UserIdentifier user, string method, object payload);
}

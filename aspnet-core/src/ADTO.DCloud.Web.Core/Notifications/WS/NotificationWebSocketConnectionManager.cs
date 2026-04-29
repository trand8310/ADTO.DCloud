using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Dependency;

namespace ADTO.DCloud.Notifications.WS;

public class NotificationWebSocketConnectionManager : INotificationWebSocketConnectionManager, ISingletonDependency
{
    private readonly ConcurrentDictionary<string, (WebSocket Socket, UserIdentifier User)> _connections = new();

    public Task<string> AddConnectionAsync(WebSocket socket, UserIdentifier user)
    {
        var connectionId = Guid.NewGuid().ToString("N");
        _connections[connectionId] = (socket, user);
        return Task.FromResult(connectionId);
    }

    public async Task RemoveConnectionAsync(string connectionId)
    {
        if (_connections.TryRemove(connectionId, out var entry) && entry.Socket.State == WebSocketState.Open)
        {
            await entry.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closed", CancellationToken.None);
        }
    }

    public async Task SendAsync(UserIdentifier user, string method, object payload)
    {
        var body = JsonSerializer.Serialize(new { method, payload });
        var buffer = Encoding.UTF8.GetBytes(body);

        foreach (var item in _connections.Where(x => x.Value.User.UserId == user.UserId && x.Value.User.TenantId == user.TenantId))
        {
            if (item.Value.Socket.State == WebSocketState.Open)
            {
                await item.Value.Socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
